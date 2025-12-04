namespace BE_Portfolio.DTOs.Auth;

public record LoginResponseDTO(
    bool RequiresTwoFactor,
    string? TempToken,
    string Username,
    string Role
);
