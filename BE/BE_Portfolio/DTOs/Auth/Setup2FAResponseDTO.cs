namespace BE_Portfolio.DTOs.Auth;

public record Setup2FAResponseDTO(
    string Secret,
    string QrCodeDataUrl,
    List<string> RecoveryCodes
);
