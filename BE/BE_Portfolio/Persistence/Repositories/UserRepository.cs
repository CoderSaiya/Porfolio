using BE_Portfolio.Models.Documents;
using BE_Portfolio.Persistence.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BE_Portfolio.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IMongoDbContext _context;

    public UserRepository(IMongoDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken ct = default)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Username, username);
        return await _context.Users.Find(filter).FirstOrDefaultAsync(ct);
    }

    public async Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        var filter = Builders<User>.Filter.Eq(u => u.Email, email);
        return await _context.Users.Find(filter).FirstOrDefaultAsync(ct);
    }

    public async Task<User?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        if (!ObjectId.TryParse(id, out var objId))
            return null;

        var filter = Builders<User>.Filter.Eq(u => u.Id, objId);
        return await _context.Users.Find(filter).FirstOrDefaultAsync(ct);
    }

    public async Task CreateAsync(User user, CancellationToken ct = default)
    {
        await _context.Users.InsertOneAsync(user, cancellationToken: ct);
    }

    public async Task UpdateRefreshTokenAsync(string userId, string? refreshToken, DateTime? expiryTime, CancellationToken ct = default)
    {
        if (!ObjectId.TryParse(userId, out var objId))
            return;

        var filter = Builders<User>.Filter.Eq(u => u.Id, objId);
        var update = Builders<User>.Update
            .Set(u => u.RefreshToken, refreshToken)
            .Set(u => u.RefreshTokenExpiryTime, expiryTime);

        await _context.Users.UpdateOneAsync(filter, update, cancellationToken: ct);
    }

    public async Task Update2FASettingsAsync(string userId, bool enabled, string? secret, List<string> recoveryCodes, CancellationToken ct = default)
    {
        if (!ObjectId.TryParse(userId, out var objId))
            return;

        var filter = Builders<User>.Filter.Eq(u => u.Id, objId);
        var update = Builders<User>.Update
            .Set(u => u.TwoFactorEnabled, enabled)
            .Set(u => u.TwoFactorSecret, secret)
            .Set(u => u.RecoveryCodes, recoveryCodes);

        await _context.Users.UpdateOneAsync(filter, update, cancellationToken: ct);
    }

    public async Task UpdateLastLoginAsync(string userId, CancellationToken ct = default)
    {
        if (!ObjectId.TryParse(userId, out var objId))
            return;

        var filter = Builders<User>.Filter.Eq(u => u.Id, objId);
        var update = Builders<User>.Update.Set(u => u.LastLoginAt, DateTime.UtcNow);

        await _context.Users.UpdateOneAsync(filter, update, cancellationToken: ct);
    }

    public async Task RemoveRecoveryCodeAsync(string userId, string code, CancellationToken ct = default)
    {
        if (!ObjectId.TryParse(userId, out var objId))
            return;

        var filter = Builders<User>.Filter.Eq(u => u.Id, objId);
        var update = Builders<User>.Update.Pull(u => u.RecoveryCodes, code);

        await _context.Users.UpdateOneAsync(filter, update, cancellationToken: ct);
    }
}
