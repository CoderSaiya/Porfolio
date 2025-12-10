namespace BE_Portfolio.Helpers;

public static class FileHelper
{
    private static readonly string[] AllowedImageTypes = { "image/jpeg", "image/png", "image/webp", "image/avif" };

    public static void ValidateImage(IFormFile? file)
    {
        if (file is null || file.Length == 0)
            throw new ArgumentException("File is empty", nameof(file));

        if (!AllowedImageTypes.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase))
            throw new InvalidOperationException($"Unsupported format: {file.ContentType}. Allowed: {string.Join(", ", AllowedImageTypes)}");
    }

    public static bool IsValidImage(IFormFile? file)
    {
        if (file is null || file.Length == 0) return false;
        return AllowedImageTypes.Contains(file.ContentType, StringComparer.OrdinalIgnoreCase);
    }
}
