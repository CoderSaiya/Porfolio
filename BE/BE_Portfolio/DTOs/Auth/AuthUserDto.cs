namespace BE_Portfolio.DTOs.Auth;

public class AuthUserDto
{
    public string Username { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public bool TwoFactorEnabled { get; set; }
}
