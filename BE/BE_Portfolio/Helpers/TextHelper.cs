using System.Text.RegularExpressions;
using System.Text;

namespace BE_Portfolio.Helpers;

public static class TextHelper
{
    public static string GenerateSlug(string title)
    {
        if (string.IsNullOrWhiteSpace(title)) return "";
        
        var str = title.ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var chars = str.Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark).ToArray();
        str = new string(chars).Normalize(NormalizationForm.FormC);

        // Replace invalid chars with hyphen
        str = Regex.Replace(str, @"[^a-z0-9\s-]", "");
        // Convert multiple spaces/hyphens to single hyphen
        str = Regex.Replace(str, @"[\s-]+", "-").Trim('-');

        return str;
    }

    public static async Task<string> ConvertFileToBase64Async(IFormFile file, CancellationToken ct = default)
    {
        if (file == null || file.Length == 0) return "";
        
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms, ct);
        var base64 = Convert.ToBase64String(ms.ToArray());
        return $"data:{file.ContentType};base64,{base64}";
    }
}
