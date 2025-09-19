using BE_Portfolio.Models.Commons;
using BE_Portfolio.Models.Documents;
using BE_Portfolio.Models.ValueObjects;
using BE_Portfolio.Persistence.Data;
using BE_Portfolio.Persistence.Repositories.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BE_Portfolio.Persistence.Repositories;

public class ImageRepository(IMongoDbContext ctx) : IImageRepository
{
    private IMongoCollection<Image> Col => ctx.Images;
    
    public async Task SetImageAsync(ImageOwnerType ownerType, ObjectId ownerId, ImageVariant variant, string mimeType, byte[] data,
        int? w, int? h, CancellationToken ct = default)
    {
        var filter = Builders<Image>.Filter.Where(x =>
            x.OwnerType == ownerType && x.OwnerId == ownerId && x.Variant == variant);

        var doc = new Image
        {
            OwnerType = ownerType,
            OwnerId = ownerId,
            Variant = variant,
            MimeType = mimeType,
            Data = data,
            Width = w,
            Height = h,
        };

        await Col.ReplaceOneAsync(filter, doc, new ReplaceOptions { IsUpsert = true }, ct);
    }

    public async Task<(string base64, string mime)?> GetBase64Async(ImageOwnerType ownerType, ObjectId ownerId, ImageVariant variant, CancellationToken ct = default)
    {
        var proj = Builders<Image>.Projection
            .Include(x => x.Data)
            .Include(x => x.MimeType);

        var cursor = await Col.Find(x => x.OwnerType == ownerType && x.OwnerId == ownerId && x.Variant == variant)
            .Project(proj)
            .FirstOrDefaultAsync(ct);
        if (cursor is null) return null;

        // Lấy Binary và MimeType từ projection
        var data = cursor.GetValue("data").AsBsonBinaryData.Bytes;
        var mime = cursor.GetValue("mime").AsString;

        var base64 = Convert.ToBase64String(data);
        return (base64, mime);
    }

    public Task DeleteAsync(ImageOwnerType ownerType, ObjectId ownerId, ImageVariant variant, CancellationToken ct = default)
        => Col.DeleteOneAsync(x => x.OwnerType == ownerType && x.OwnerId == ownerId && x.Variant == variant, ct);
}