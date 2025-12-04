using BE_Portfolio.Models.Documents;

namespace BE_Portfolio.Services.TwoFactor;

public interface ITwoFactorService
{
    string GenerateSecret();
    string GenerateQrCodeUri(string username, string secret, string issuer = "Portfolio");
    string GenerateQrCodeDataUrl(string uri);
    bool ValidateCode(string secret, string code);
    List<string> GenerateRecoveryCodes(int count = 10);
    bool ValidateRecoveryCode(User user, string code);
}
