using BE_Portfolio.Models.Documents;
using OtpNet;
using QRCoder;
using System.Security.Cryptography;

namespace BE_Portfolio.Services.TwoFactor;

public class TwoFactorService : ITwoFactorService
{
    public string GenerateSecret()
    {
        var key = KeyGeneration.GenerateRandomKey(20); // 160 bits
        return Base32Encoding.ToString(key);
    }

    public string GenerateQrCodeUri(string username, string secret, string issuer = "Portfolio")
    {
        return $"otpauth://totp/{issuer}:{username}?secret={secret}&issuer={issuer}";
    }

    public string GenerateQrCodeDataUrl(string uri)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrCodeData = qrGenerator.CreateQrCode(uri, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new PngByteQRCode(qrCodeData);
        var qrCodeImage = qrCode.GetGraphic(20);
        return $"data:image/png;base64,{Convert.ToBase64String(qrCodeImage)}";
    }

    public bool ValidateCode(string secret, string code)
    {
        try
        {
            var secretBytes = Base32Encoding.ToBytes(secret);
            var totp = new Totp(secretBytes);
            
            // Verify code with time window (allows for clock skew)
            return totp.VerifyTotp(code, out _, new VerificationWindow(1, 1));
        }
        catch
        {
            return false;
        }
    }

    public List<string> GenerateRecoveryCodes(int count = 10)
    {
        var codes = new List<string>();
        
        for (int i = 0; i < count; i++)
        {
            var bytes = RandomNumberGenerator.GetBytes(8);
            var code = Convert.ToBase64String(bytes)
                .Replace("+", "")
                .Replace("/", "")
                .Replace("=", "")
                .Substring(0, 10)
                .ToUpper();
            
            // Format as XXXXX-XXXXX
            code = $"{code.Substring(0, 5)}-{code.Substring(5, 5)}";
            codes.Add(code);
        }
        
        return codes;
    }

    public bool ValidateRecoveryCode(User user, string code)
    {
        return user.RecoveryCodes.Contains(code, StringComparer.OrdinalIgnoreCase);
    }
}
