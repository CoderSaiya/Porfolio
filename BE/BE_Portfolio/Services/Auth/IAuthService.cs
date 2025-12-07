using System.Security.Claims;
using BE_Portfolio.DTOs.Auth;
using BE_Portfolio.Models.Documents;
using Microsoft.AspNetCore.Http;

namespace BE_Portfolio.Services.Auth;

public interface IAuthService
{
    Task<LoginResponseDTO> LoginAsync(string username, string password,HttpResponse response, CancellationToken ct = default);
    Task<bool> RegisterAsync(RegisterRequestDTO request, CancellationToken ct = default);
    Task<bool> Verify2FAAndSetCookiesAsync(string tempToken, string code, HttpResponse response, CancellationToken ct = default);
    Task<bool> RefreshTokenAsync(string refreshToken, HttpResponse response, CancellationToken ct = default);
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    void SetAuthCookies(HttpResponse response, string accessToken, string refreshToken);
    void ClearAuthCookies(HttpResponse response);
    Task<string?> GetUserIdFromTokenAsync(string token);
    Task<User> FindOrCreateExternal(string provider, string providerKey, string? email, ClaimsPrincipal principal);
    Task<IssueTokenResponseDTO> IssueTokensForUserAsync(User user);
    Task<AuthUserDto> GetCurrentUserAsync(string userId, CancellationToken ct = default);
}
