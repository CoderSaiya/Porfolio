using System.ComponentModel.DataAnnotations;
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

public class AuthService(
    IUserRepository userRepo,
    IPasswordHasher passwordHasher,
    ITwoFactorService twoFactorService,
    JwtSettings jwtSettings,
    CookieSettings cookieSettings)
    : IAuthService
{
    public async Task<bool> RegisterAsync(RegisterRequestDTO request, CancellationToken ct = default)
    {
        var existingUser = await userRepo.GetByUsernameAsync(request.Username, ct);
        if (existingUser != null)
        {
            return false; // User already exists
        }

        var existingEmail = await userRepo.GetByEmailAsync(request.Email, ct);
        if (existingEmail != null)
        {
            return false;
        }

        var newUser = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = passwordHasher.HashPassword(request.Password),
            FullName = request.FullName,
            Role = "User"
        };

        try
        {
            await userRepo.CreateAsync(newUser, ct);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<LoginResponseDTO> LoginAsync(string username, string password, HttpResponse response, CancellationToken ct = default)
    {
        var user = await userRepo.GetByUsernameAsync(username, ct);
        
        if (user == null || !passwordHasher.VerifyPassword(password, user.PasswordHash))
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        // Nếu bật 2FA → chỉ trả temp token, chưa set cookie
        if (user.TwoFactorEnabled)
        {
            var tempToken = GenerateTempToken(user);
            return new LoginResponseDTO(
                Id: user.Id.ToString(),
                RequiresTwoFactor: true,
                TempToken: tempToken,
                Username: user.Username,
                Role: user.Role
            );
        }

        // Nếu KHÔNG bật 2FA → login full luôn: tạo token + set cookie
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();

        var refreshTokenExpiry = DateTime.UtcNow.AddDays(jwtSettings.RefreshTokenExpirationDays);
        await userRepo.UpdateRefreshTokenAsync(user.Id.ToString(), refreshToken, refreshTokenExpiry, ct);
        await userRepo.UpdateLastLoginAsync(user.Id.ToString(), ct);

        SetAuthCookies(response, accessToken, refreshToken);
        
        return new LoginResponseDTO(
            Id: user.Id.ToString(),
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

            var user = await userRepo.GetByIdAsync(userId, ct);
            if (user == null || !user.TwoFactorEnabled || string.IsNullOrEmpty(user.TwoFactorSecret))
                return false;

            // Validate TOTP code
            if (!twoFactorService.ValidateCode(user.TwoFactorSecret, code))
            {
                // Check if it's a recovery code
                if (!twoFactorService.ValidateRecoveryCode(user, code))
                    return false;

                // Consume recovery code
                await userRepo.RemoveRecoveryCodeAsync(userId, code, ct);
            }

            // Generate tokens
            var accessToken = GenerateAccessToken(user);
            var refreshToken = GenerateRefreshToken();

            // Save refresh token to database
            var refreshTokenExpiry = DateTime.UtcNow.AddDays(jwtSettings.RefreshTokenExpirationDays);
            await userRepo.UpdateRefreshTokenAsync(userId, refreshToken, refreshTokenExpiry, ct);
            await userRepo.UpdateLastLoginAsync(userId, ct);

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
            var users = await userRepo.GetByIdAsync("", ct); // We need to add FindByRefreshToken method
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
        var key = Encoding.UTF8.GetBytes(jwtSettings.Secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            }),
            Expires = DateTime.UtcNow.AddMinutes(jwtSettings.AccessTokenExpirationMinutes),
            Issuer = jwtSettings.Issuer,
            Audience = jwtSettings.Audience,
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
        var sameSiteMode = cookieSettings.SameSite.ToLower() switch
        {
            "strict" => SameSiteMode.Strict,
            "lax" => SameSiteMode.Lax,
            "none" => SameSiteMode.None,
            _ => SameSiteMode.Strict
        };

        // Access token cookie
        response.Cookies.Append(cookieSettings.AccessTokenCookieName, accessToken, new CookieOptions
        {
            HttpOnly = cookieSettings.HttpOnly,
            Secure = cookieSettings.Secure,
            SameSite = sameSiteMode,
            Expires = DateTimeOffset.UtcNow.AddMinutes(jwtSettings.AccessTokenExpirationMinutes)
        });

        // Refresh token cookie
        response.Cookies.Append(cookieSettings.RefreshTokenCookieName, refreshToken, new CookieOptions
        {
            HttpOnly = cookieSettings.HttpOnly,
            Secure = cookieSettings.Secure,
            SameSite = sameSiteMode,
            Expires = DateTimeOffset.UtcNow.AddDays(jwtSettings.RefreshTokenExpirationDays)
        });
    }

    public void ClearAuthCookies(HttpResponse response)
    {
        response.Cookies.Delete(cookieSettings.AccessTokenCookieName);
        response.Cookies.Delete(cookieSettings.RefreshTokenCookieName);
    }

    public async Task<string?> GetUserIdFromTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(jwtSettings.Secret);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,
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
    
    public async Task<User> FindOrCreateExternal(string provider, string providerKey, string? email, ClaimsPrincipal principal)
    {
        User? user = null;
        if (!string.IsNullOrWhiteSpace(email))
        {
            user = await userRepo.GetByEmailAsync(email);
            if (user is not null) return user;
        }
        
        if (string.IsNullOrWhiteSpace(email))
            throw new ValidationException("Provider không trả về email. Vui lòng bật email public hoặc đăng nhập cách khác.");
        
        // Create user
        var newUser = new User
        {
            Email = email,
            Username = email!.Split('@')[0], // Generate simple username
            PasswordHash = passwordHasher.HashPassword(Guid.NewGuid().ToString("N")),
            AvatarUrl = principal.FindFirst("picture")?.Value ?? principal.FindFirst("avatar_url")?.Value
        };
        
        var givenName = principal.FindFirst(ClaimTypes.GivenName)?.Value ?? "";
        var surname = principal.FindFirst(ClaimTypes.Surname)?.Value ?? "";
        var parts = new[] { givenName, surname }.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray();
        
        newUser.FullName = string.Join(" ", parts);

        await userRepo.CreateAsync(newUser);

        return newUser;
    }

    public async Task<IssueTokenResponseDTO> IssueTokensForUserAsync(User user)
    {
        var accessToken = GenerateAccessToken(user);
        var refreshToken = GenerateRefreshToken();
        
        var refreshTokenExpiry = DateTime.UtcNow.AddDays(jwtSettings.RefreshTokenExpirationDays);
        await userRepo.UpdateRefreshTokenAsync(user.Id.ToString(), refreshToken, refreshTokenExpiry);
        await userRepo.UpdateLastLoginAsync(user.Id.ToString());
        
        return new IssueTokenResponseDTO(accessToken, refreshToken, DateTimeOffset.UtcNow.AddMinutes(jwtSettings.AccessTokenExpirationMinutes));
    }

    public async Task<AuthUserDto> GetCurrentUserAsync(string userId, CancellationToken ct = default)
    {
         var user = await userRepo.GetByIdAsync(userId, ct);
         if (user == null) throw new UnauthorizedAccessException();
         
         return new AuthUserDto
         {
             Id = user.Id.ToString(),
             Username = user.Username,
             Role = user.Role,
             FullName = user.FullName,
             AvatarUrl = user.AvatarUrl,
             TwoFactorEnabled = user.TwoFactorEnabled
         };
    }

    private string GenerateTempToken(User user)
    {
        // Generate a short-lived temp token (5 minutes) for 2FA verification
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(jwtSettings.Secret);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim("temp_2fa", "true")
            }),
            Expires = DateTime.UtcNow.AddMinutes(5),
            Issuer = jwtSettings.Issuer,
            Audience = jwtSettings.Audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}
