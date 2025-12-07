using BE_Portfolio.Models.Documents;

namespace BE_Portfolio.Persistence.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> GetByIdAsync(string id, CancellationToken ct = default);
    Task CreateAsync(User user, CancellationToken ct = default);
    Task UpdateRefreshTokenAsync(string userId, string? refreshToken, DateTime? expiryTime, CancellationToken ct = default);
    Task Update2FASettingsAsync(string userId, bool enabled, string? secret, List<string> recoveryCodes, CancellationToken ct = default);
    Task UpdateLastLoginAsync(string userId, CancellationToken ct = default);
    Task RemoveRecoveryCodeAsync(string userId, string code, CancellationToken ct = default);
}
