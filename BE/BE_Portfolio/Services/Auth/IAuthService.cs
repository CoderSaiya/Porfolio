using BE_Portfolio.DTOs.Auth;
using BE_Portfolio.Models.Documents;
using Microsoft.AspNetCore.Http;

namespace BE_Portfolio.Services.Auth;

public interface IAuthService
{
    Task<LoginResponseDTO> LoginAsync(string username, string password, CancellationToken ct = default);
    Task<bool> Verify2FAAndSetCookiesAsync(string tempToken, string code, HttpResponse response, CancellationToken ct = default);
    Task<bool> RefreshTokenAsync(string refreshToken, HttpResponse response, CancellationToken ct = default);
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    void SetAuthCookies(HttpResponse response, string accessToken, string refreshToken);
    void ClearAuthCookies(HttpResponse response);
    Task<string?> GetUserIdFromTokenAsync(string token);
}
