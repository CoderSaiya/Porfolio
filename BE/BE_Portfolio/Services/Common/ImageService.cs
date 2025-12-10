using BE_Portfolio.Helpers;
using BE_Portfolio.Models.ValueObjects;
using BE_Portfolio.Persistence.Repositories.Interfaces;
using MongoDB.Bson;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Webp;
using Size = SixLabors.ImageSharp.Size;

namespace BE_Portfolio.Services.Common;

public class ImageService(IImageRepository imageRepo) : IImageService
{
    public async Task ProcessAndSaveAsync(ImageOwnerType ownerType, string ownerId, ImageVariant variant, IFormFile file, CancellationToken ct = default)
    {
        FileHelper.ValidateImage(file);

        if (!ObjectId.TryParse(ownerId, out var parsedId))
            throw new ArgumentException("Invalid Owner ID", nameof(ownerId));

        await using var input = file.OpenReadStream();
        using var image = await Image.LoadAsync(input, ct);
        image.Mutate(x => x.AutoOrient());

        // Determine resize/optimization logic based on variant/type if needed
        // For now using the logic from PortfolioService
        
        int targetWidth = 1920;
        int targetHeight = 1920;
        int quality = 80;

        if (variant == ImageVariant.Thumb)
        {
            if (ownerType == ImageOwnerType.Profile)
            {
                targetWidth = 256;
                targetHeight = 256;
            }
            else
            {
                targetWidth = 720;
                targetHeight = 720;
            }
            quality = 82;
        }
        else // Full
        {
            if (ownerType == ImageOwnerType.Profile)
            {
                targetWidth = 1024;
                targetHeight = 1024;
            }
        }

        using var processedImg = image.Clone(ctx => ctx.Resize(new ResizeOptions
        {
            Mode = ResizeMode.Max,
            Size = new Size(targetWidth, targetHeight),
            Sampler = KnownResamplers.Lanczos3,
            Compand = true
        }));

        using var ms = new MemoryStream();
        await processedImg.SaveAsync(ms, new WebpEncoder { Quality = quality }, ct);
        var bytes = ms.ToArray();

        await imageRepo.SetImageAsync(
            ownerType, parsedId, variant,
            "image/webp", bytes, processedImg.Width, processedImg.Height, ct);
    }

    public async Task<(byte[] Bytes, string MimeType)?> GetImageBytesAsync(ImageOwnerType ownerType, string ownerId, ImageVariant variant, CancellationToken ct = default)
    {
        if (!ObjectId.TryParse(ownerId, out var parsedId)) return null;

        var res = await imageRepo.GetBase64Async(ownerType, parsedId, variant, ct);
        if (res is null) return null;
        
        var (base64, mime) = res.Value;
        return (Convert.FromBase64String(base64), mime);
    }

    public async Task<string?> GetImageDataUrlAsync(ImageOwnerType ownerType, string ownerId, ImageVariant variant, CancellationToken ct = default)
    {
        if (!ObjectId.TryParse(ownerId, out var parsedId)) return null;

        var res = await imageRepo.GetBase64Async(ownerType, parsedId, variant, ct);
        if (res is null) return null;

        var (base64, mime) = res.Value;
        return $"data:{mime};base64,{base64}";
    }

    public async Task DeleteImageAsync(ImageOwnerType ownerType, string ownerId, ImageVariant variant, CancellationToken ct = default)
    {
        if (!ObjectId.TryParse(ownerId, out var parsedId)) return;
        await imageRepo.DeleteAsync(ownerType, parsedId, variant, ct);
    }
}
