using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BE_Portfolio.Configuration;
using BE_Portfolio.DTOs.Auth;
using BE_Portfolio.Models.Documents;
using BE_Portfolio.Persistence.Repositories;
using BE_Portfolio.Services.TwoFactor;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;

namespace BE_Portfolio.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepo;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITwoFactorService _twoFactorService;
    private readonly JwtSettings _jwtSettings;
    private readonly CookieSettings _cookieSettings;

    public AuthService(
        IUserRepository userRepo,
        IPasswordHasher passwordHasher,
        ITwoFactorService twoFactorService,
        JwtSettings jwtSettings,
        CookieSettings cookieSettings)
    {
        _userRepo = userRepo;
        _passwordHasher = passwordHasher;
        _twoFactorService = twoFactorService;
        _jwtSettings = jwtSettings;
        _cookieSettings = cookieSettings;
    }

    public async Task<LoginResponseDTO> LoginAsync(string username, string password, CancellationToken ct = default)
    {
        var user = await _userRepo.GetByUsernameAsync(username, ct);
        
        if (user == null || !_passwordHasher.VerifyPassword(password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        // If 2FA is enabled, return temp token
        if (user.TwoFactorEnabled)
        {
            var tempToken = GenerateTempToken(user);
            return new LoginResponseDTO(
                RequiresTwoFactor: true,
                TempToken: tempToken,
                Username: user.Username,
                Role: user.Role
            );
        }

        // If 2FA is not enabled, this shouldn't happen in production
        // but we can handle it by returning success without 2FA
        return new LoginResponseDTO(
            RequiresTwoFactor: false,
            TempToken: null,
            Username: user.Username,
            Role: user.Role
        );
    }

    public async Task<bool> Verify2FAAndSetCookiesAsync(string tempToken, string code, HttpResponse response, CancellationToken ct = default)
    {
        try
        {
            var userId = await GetUserIdFromTokenAsync(tempToken);
            if (userId == null) return false;

            var user = await _userRepo.GetByIdAsync(userId, ct);
            if (user == null || !user.TwoFactorEnabled || string.IsNullOrEmpty(user.TwoFactorSecret))
                return false;

            // Validate TOTP code
            if (!_twoFactorService.ValidateCode(user.TwoFactorSecret, code))
            {
                // Check if it's a recovery code
                if (!_twoFactorService.ValidateRecoveryCode(user, code))
                    return false;

                // Consume recovery code
                await _userRepo.RemoveRecoveryCodeAsync(userId, code, ct);
            }

            // Generate tokens
            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            // Save refresh token to database
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
            await _userRepo.UpdateRefreshTokenAsync(userId, refreshToken, refreshTokenExpiry, ct);
            await _userRepo.UpdateLastLoginAsync(userId, ct);

            // Set httpOnly cookies
            SetAuthCookies(response, accessToken, refreshToken);

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RefreshTokenAsync(string refreshToken, HttpResponse response, CancellationToken ct = default)
    {
        try
        {
            // Find user by refresh token
            var users = await _userRepo.GetByIdAsync("", ct); // We need to add FindByRefreshToken method
            // For now, we'll validate the token structure only
            
            if (string.IsNullOrEmpty(refreshToken))
                return false;

            // TODO: Implement proper refresh token validation
            // This is a simplified version - in production, store refresh tokens with user association

            return false; // Placeholder
        }
        catch
        {
            return false;
        }
    }

    public string GenerateAccessToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public void SetAuthCookies(HttpResponse response, string accessToken, string refreshToken)
    {
        var sameSiteMode = _cookieSettings.SameSite.ToLower() switch
        {
            "strict" => SameSiteMode.Strict,
            "lax" => SameSiteMode.Lax,
            "none" => SameSiteMode.None,
            _ => SameSiteMode.Strict
        };

        // Access token cookie
        response.Cookies.Append(_cookieSettings.AccessTokenCookieName, accessToken, new CookieOptions
        {
            HttpOnly = _cookieSettings.HttpOnly,
            Secure = _cookieSettings.Secure,
            SameSite = sameSiteMode,
            Expires = DateTimeOffset.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes)
        });

        // Refresh token cookie
        response.Cookies.Append(_cookieSettings.RefreshTokenCookieName, refreshToken, new CookieOptions
        {
            HttpOnly = _cookieSettings.HttpOnly,
            Secure = _cookieSettings.Secure,
            SameSite = sameSiteMode,
            Expires = DateTimeOffset.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays)
        });
    }

    public void ClearAuthCookies(HttpResponse response)
    {
        response.Cookies.Delete(_cookieSettings.AccessTokenCookieName);
        response.Cookies.Delete(_cookieSettings.RefreshTokenCookieName);
    }

    public async Task<string?> GetUserIdFromTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);

            return await Task.FromResult(userIdClaim?.Value);
        }
        catch
        {
            return null;
        }
    }

    private string GenerateTempToken(User user)
    {
        // Generate a short-lived temp token (5 minutes) for 2FA verification
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("temp_2fa", "true")
            }),
            Expires = DateTime.UtcNow.AddMinutes(5),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
