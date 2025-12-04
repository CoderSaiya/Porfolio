namespace BE_Portfolio.Configuration;

public class CookieSettings
{
    public string AccessTokenCookieName { get; set; } = "access_token";
    public string RefreshTokenCookieName { get; set; } = "refresh_token";
    public string SameSite { get; set; } = "Strict";
    public bool Secure { get; set; } = true;
    public bool HttpOnly { get; set; } = true;
}
