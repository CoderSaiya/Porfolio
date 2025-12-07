namespace BE_Portfolio.DTOs.Auth;

public record LoginResponseDTO(
    string Id,
    bool RequiresTwoFactor,
    string? TempToken,
    string Username,
    string Role
);
