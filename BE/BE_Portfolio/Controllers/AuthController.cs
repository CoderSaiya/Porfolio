using System.Security.Claims;
using BE_Portfolio.DTOs.Admin;
using BE_Portfolio.DTOs.Auth;
using BE_Portfolio.Persistence.Repositories;
using BE_Portfolio.Services.Auth;
using BE_Portfolio.Services.TwoFactor;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE_Portfolio.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IUserRepository _userRepo;
    private readonly ITwoFactorService _twoFactorService;

    public AuthController(
        IAuthService authService,
        IUserRepository userRepo,
        ITwoFactorService twoFactorService)
    {
        _authService = authService;
        _userRepo = userRepo;
        _twoFactorService = twoFactorService;
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me()
    {
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;

        if (username == null)
            return Unauthorized();

        var user = new AuthUserDto
        {
            Username = username,
            Role = role ?? "User"
        };

        return Ok(user);
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO request, CancellationToken ct)
    {
        try
        {
            var result = await _authService.LoginAsync(request.Username, request.Password, Response, ct);
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

    [HttpPost("verify-2fa")]
    public async Task<IActionResult> Verify2FA([FromBody] Verify2FARequestDTO request, [FromQuery] string tempToken, CancellationToken ct)
    {
        try
        {
            var success = await _authService.Verify2FAAndSetCookiesAsync(tempToken, request.Code, Response, ct);
            
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

            var success = await _authService.RefreshTokenAsync(refreshToken, Response, ct);
            
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
        _authService.ClearAuthCookies(Response);
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

            var user = await _userRepo.GetByIdAsync(userId, ct);
            if (user == null)
                return NotFound();

            // Generate secret and QR code
            var secret = _twoFactorService.GenerateSecret();
            var qrCodeUri = _twoFactorService.GenerateQrCodeUri(user.Username, secret);
            var qrCodeDataUrl = _twoFactorService.GenerateQrCodeDataUrl(qrCodeUri);
            var recoveryCodes = _twoFactorService.GenerateRecoveryCodes();

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
            if (!_twoFactorService.ValidateCode(secret, request.Code))
                return BadRequest(new { message = "Invalid 2FA code" });

            // Parse recovery codes
            var codes = recoveryCodes.Split(',').ToList();

            // Save 2FA settings
            await _userRepo.Update2FASettingsAsync(userId, true, secret, codes, ct);

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

            var user = await _userRepo.GetByIdAsync(userId, ct);
            if (user == null || !user.TwoFactorEnabled || string.IsNullOrEmpty(user.TwoFactorSecret))
                return BadRequest(new { message = "2FA is not enabled" });

            // Verify current 2FA code before disabling
            if (!_twoFactorService.ValidateCode(user.TwoFactorSecret, request.Code))
                return BadRequest(new { message = "Invalid 2FA code" });

            // Disable 2FA
            await _userRepo.Update2FASettingsAsync(userId, false, null, new List<string>(), ct);

            return Ok(new { message = "2FA disabled successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while disabling 2FA", error = ex.Message });
        }
    }
}
