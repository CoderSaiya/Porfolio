using BE_Portfolio.Models.ValueObjects;

namespace BE_Portfolio.Services.Common;

public interface IImageService
{
    Task ProcessAndSaveAsync(ImageOwnerType ownerType, string ownerId, ImageVariant variant, IFormFile file, CancellationToken ct = default);
    Task<(byte[] Bytes, string MimeType)?> GetImageBytesAsync(ImageOwnerType ownerType, string ownerId, ImageVariant variant, CancellationToken ct = default);
    Task<string?> GetImageDataUrlAsync(ImageOwnerType ownerType, string ownerId, ImageVariant variant, CancellationToken ct = default);
    Task DeleteImageAsync(ImageOwnerType ownerType, string ownerId, ImageVariant variant, CancellationToken ct = default);
}
