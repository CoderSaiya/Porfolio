namespace BE_Portfolio.DTOs.Auth;

public record IssueTokenResponseDTO(
    string AccessToken,
    string RefreshToken,
    DateTimeOffset AccessExpiresAt
);
