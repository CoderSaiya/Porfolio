using System.Net;
using System.Security.Claims;
using System.Text.Json;
using BE_Portfolio.DTOs.Admin;
using BE_Portfolio.DTOs.Auth;
using BE_Portfolio.Persistence.Repositories;
using BE_Portfolio.Services.Auth;
using BE_Portfolio.Services.TwoFactor;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using AuthUserDto = BE_Portfolio.DTOs.Auth.AuthUserDto;

namespace BE_Portfolio.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IAuthService authService,
    IUserRepository userRepo,
    ITwoFactorService twoFactorService,
    IHostEnvironment env)
    : ControllerBase
{
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> Me(CancellationToken ct)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null) return Unauthorized();

        var user = await userRepo.GetByIdAsync(userId, ct);
        if (user == null) return Unauthorized();

        var authUser = new AuthUserDto
        {
            Username = user.Username,
            Role = user.Role,
            FullName = user.FullName,
            AvatarUrl = user.AvatarUrl,
            TwoFactorEnabled = user.TwoFactorEnabled
        };

        return Ok(authUser);
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var result = await authService.RegisterAsync(request, ct);
            if (!result)
                return BadRequest(new { message = "Username or Email already exists" });

            return Ok(new { message = "Registration successful" });
        }
        catch (Exception ex)
        {
             return StatusCode(500, new { message = "An error occurred during registration", error = ex.Message });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO request, CancellationToken ct)
    {
        try
        {
            var result = await authService.LoginAsync(request.Username, request.Password, Response, ct);
            
            // Get user details to return FullName/Avatar
            if (!result.RequiresTwoFactor) {
                 var user = await userRepo.GetByUsernameAsync(result.Username, ct);
                 if (user != null) {
                     return Ok(new {
                         result.RequiresTwoFactor,
                         result.TempToken,
                         result.Username,
                         result.Role,
                         FullName = user.FullName,
                         AvatarUrl = user.AvatarUrl
                     });
                 }
            }
            
            return Ok(result);
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new { message = "Invalid credentials" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred during login", error = ex.Message });
        }
    }

    [HttpGet("external/callback")]
    [AllowAnonymous]
    public async Task<IActionResult> ExternalCallback(
        [FromQuery] string? returnUrl,
        [FromServices] IConfiguration cfg)
    {
        var result = await HttpContext.AuthenticateAsync("External");
        if (!result.Succeeded)
            return Redirect($"{cfg["FrontendBaseUrl"]}/auth/login?error=external_failed");

        var principal = result.Principal!;
        var email = principal.FindFirst(ClaimTypes.Email)?.Value;
        var providerKey = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var provider = result.Ticket!.AuthenticationScheme; // "Google" | "GitHub"

        if (string.IsNullOrEmpty(providerKey))
            return Redirect($"{cfg["FrontendBaseUrl"]}/auth/login?error=missing_provider_key");

        var user = await authService.FindOrCreateExternal(provider, providerKey, email, principal);
        var issued = await authService.IssueTokensForUserAsync(user);

        // Use SetAuthCookies for consistency
        authService.SetAuthCookies(Response, issued.AccessToken, issued.RefreshToken);
        
        var userDto = await authService.GetCurrentUserAsync(user.Id.ToString());
        var payload = new { user = userDto };
        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        var b64 = WebEncoders.Base64UrlEncode(System.Text.Encoding.UTF8.GetBytes(json));

        // Redirect to success page
        var successUrl = string.IsNullOrEmpty(returnUrl)
            ? $"{cfg["FrontendBaseUrl"]}/auth/sso/success"
            : returnUrl!;
        var redirectWithPayload = $"{successUrl}#auth={b64}";

        await HttpContext.SignOutAsync("External");
        return Redirect(redirectWithPayload);
    }

    [HttpPost("verify-2fa")]
    public async Task<IActionResult> Verify2FA([FromBody] Verify2FARequestDTO request, [FromQuery] string tempToken, CancellationToken ct)
    {
        try
        {
            var success = await authService.Verify2FAAndSetCookiesAsync(tempToken, request.Code, Response, ct);
            
            if (!success)
                return Unauthorized(new { message = "Invalid 2FA code" });

            return Ok(new { message = "Authentication successful" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred during 2FA verification", error = ex.Message });
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> RefreshToken(CancellationToken ct)
    {
        try
        {
            var refreshToken = Request.Cookies["refresh_token"];
            
            if (string.IsNullOrEmpty(refreshToken))
                return Unauthorized(new { message = "No refresh token provided" });

            var success = await authService.RefreshTokenAsync(refreshToken, Response, ct);
            
            if (!success)
                return Unauthorized(new { message = "Invalid refresh token" });

            return Ok(new { message = "Token refreshed successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred during token refresh", error = ex.Message });
        }
    }

    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout()
    {
        authService.ClearAuthCookies(Response);
        return Ok(new { message = "Logged out successfully" });
    }

    [HttpGet("2fa/setup")]
    [Authorize]
    public async Task<IActionResult> Setup2FA(CancellationToken ct)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var user = await userRepo.GetByIdAsync(userId, ct);
            if (user == null)
                return NotFound();

            // Generate secret and QR code
            var secret = twoFactorService.GenerateSecret();
            var qrCodeUri = twoFactorService.GenerateQrCodeUri(user.Username, secret);
            var qrCodeDataUrl = twoFactorService.GenerateQrCodeDataUrl(qrCodeUri);
            var recoveryCodes = twoFactorService.GenerateRecoveryCodes();

            // Don't save yet - user needs to verify first
            var response = new Setup2FAResponseDTO(secret, qrCodeDataUrl, recoveryCodes);
            return Ok(response);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred during 2FA setup", error = ex.Message });
        }
    }

    [HttpPost("2fa/enable")]
    [Authorize]
    public async Task<IActionResult> Enable2FA([FromBody] Verify2FARequestDTO request, [FromQuery] string secret, [FromQuery] string recoveryCodes, CancellationToken ct)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            // Validate the code with the provided secret
            if (!twoFactorService.ValidateCode(secret, request.Code))
                return BadRequest(new { message = "Invalid 2FA code" });

            // Parse recovery codes
            var codes = recoveryCodes.Split(',').ToList();

            // Save 2FA settings
            await userRepo.Update2FASettingsAsync(userId, true, secret, codes, ct);

            return Ok(new { message = "2FA enabled successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while enabling 2FA", error = ex.Message });
        }
    }

    [HttpPost("2fa/disable")]
    [Authorize]
    public async Task<IActionResult> Disable2FA([FromBody] Verify2FARequestDTO request, CancellationToken ct)
    {
        try
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userId == null)
                return Unauthorized();

            var user = await userRepo.GetByIdAsync(userId, ct);
            if (user == null || !user.TwoFactorEnabled || string.IsNullOrEmpty(user.TwoFactorSecret))
                return BadRequest(new { message = "2FA is not enabled" });

            // Verify current 2FA code before disabling
            if (!twoFactorService.ValidateCode(user.TwoFactorSecret, request.Code))
                return BadRequest(new { message = "Invalid 2FA code" });

            // Disable 2FA
            await userRepo.Update2FASettingsAsync(userId, false, null, new List<string>(), ct);

            return Ok(new { message = "2FA disabled successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while disabling 2FA", error = ex.Message });
        }
    }

    [HttpGet("external/{provider}/start")]
    [AllowAnonymous]
    public IActionResult External(
        string provider,
        [FromQuery] string? returnUrl,
        [FromServices] IConfiguration cfg)
    {
        var providers = new[] { "google", "github" };
        if (!providers.Contains(provider.ToLower()))
            return BadRequest("Unsupported provider");

        var fallbackReturn = $"{cfg["FrontendBaseUrl"]}/auth/sso/success";
        var encoded = WebUtility.UrlEncode(returnUrl ?? fallbackReturn);

        var redirectUri = $"/api/auth/external/callback?returnUrl={encoded}";
        var props = new AuthenticationProperties { RedirectUri = redirectUri };

        var scheme = provider.Equals("google", StringComparison.OrdinalIgnoreCase) ? "Google" : "GitHub";
        // Không cần mảng; 1 scheme là đủ
        return Challenge(props, scheme);
    }
}
