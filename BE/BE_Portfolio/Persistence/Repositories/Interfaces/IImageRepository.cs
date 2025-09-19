using BE_Portfolio.Models.ValueObjects;
using MongoDB.Bson;

namespace BE_Portfolio.Persistence.Repositories.Interfaces;

public interface IImageRepository
{
    Task SetImageAsync(ImageOwnerType ownerType, ObjectId ownerId, ImageVariant variant, string mimeType, byte[] data, int? w, int? h, CancellationToken ct = default);
    Task<(string base64, string mime)?> GetBase64Async(ImageOwnerType ownerType, ObjectId ownerId, ImageVariant variant, CancellationToken ct = default);
    Task DeleteAsync(ImageOwnerType ownerType, ObjectId ownerId, ImageVariant variant, CancellationToken ct = default);
}