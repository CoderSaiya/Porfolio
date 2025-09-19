using BE_Portfolio.Models.Documents;
using BE_Portfolio.Models.ValueObjects;
using BE_Portfolio.Persistence.Data;
using BE_Portfolio.Persistence.Repositories.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BE_Portfolio.Persistence.Repositories;

public class ContactMessageRepository(IMongoDbContext ctx) : IContactMessageRepository
{
    [Obsolete("Obsolete")]
    public async Task InsertAsync(ContactMessage doc, CancellationToken ct = default)
        => await ctx.ContactMessages.InsertOneAsync(doc, ct);

    public async Task<List<ContactMessage>> ListAsync(MessageStatus? status, int? limit, CancellationToken ct = default)
    {
        var filter = status.HasValue
            ? Builders<ContactMessage>.Filter.Eq(x => x.Status, status.Value)
            : Builders<ContactMessage>.Filter.Empty;

        IFindFluent<ContactMessage, ContactMessage> find = ctx.ContactMessages
            .Find(filter)
            .SortByDescending(x => x.CreatedAt);
        if (limit is > 0) find = find.Limit(limit);
        return await find.ToListAsync(ct);
    }

    public Task UpdateStatusAsync(string id, MessageStatus status, CancellationToken ct = default)
    {
        var oid = ObjectId.Parse(id);
        var upd = Builders<ContactMessage>.Update
            .Set(x => x.Status, status)
            .Set(x => x.ReadAt, status == MessageStatus.Read ? DateTime.UtcNow : null);
        return ctx.ContactMessages.UpdateOneAsync(x => x.Id == oid, upd, cancellationToken: ct);
    }
}