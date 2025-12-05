using BE_Portfolio.Models.Commons;
using MongoDB.Bson.Serialization.Attributes;

namespace BE_Portfolio.Models.Documents;

[BsonIgnoreExtraElements]
public class User : BaseDocument
{
    [BsonElement("username")]
    public string Username { get; set; } = null!;
    
    [BsonElement("email")]
    public string Email { get; set; } = null!;
    
    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } = null!;
    
    [BsonElement("role")]
    public string Role { get; set; } = "Admin";
    
    [BsonElement("lastLoginAt")]
    public DateTime? LastLoginAt { get; set; }
    
    [BsonElement("refreshToken")]
    public string? RefreshToken { get; set; }
    
    [BsonElement("refreshTokenExpiryTime")]
    public DateTime? RefreshTokenExpiryTime { get; set; }
    
    // 2FA Fields
    [BsonElement("twoFactorEnabled")]
    public bool TwoFactorEnabled { get; set; } = false;
    
    [BsonElement("twoFactorSecret")]
    public string? TwoFactorSecret { get; set; } // Base32 encoded secret for TOTP
    
    [BsonElement("recoveryCodes")]
    public List<string> RecoveryCodes { get; set; } = new();
}
