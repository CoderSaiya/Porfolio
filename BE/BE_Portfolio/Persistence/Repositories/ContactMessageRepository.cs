using BE_Portfolio.Models.Documents;
using BE_Portfolio.Models.ValueObjects;
using BE_Portfolio.Persistence.Data;
using BE_Portfolio.Persistence.Repositories.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;

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

    public async Task<ContactMessage?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        if (!ObjectId.TryParse(id, out var oid)) return null;
        return await ctx.ContactMessages.Find(x => x.Id == oid).FirstOrDefaultAsync(ct);
    }

    public async Task DeleteAsync(string id, CancellationToken ct = default)
    {
        if (!ObjectId.TryParse(id, out var oid)) return;
        await ctx.ContactMessages.DeleteOneAsync(x => x.Id == oid, ct);
    }

    public async Task BulkDeleteAsync(List<string> ids, CancellationToken ct = default)
    {
        var oids = ids.Select(ObjectId.Parse).ToList();
        await ctx.ContactMessages.DeleteManyAsync(x => oids.Contains(x.Id), ct);
    }

    public async Task<long> CountAsync(MessageStatus? status, string? searchTerm, CancellationToken ct = default)
    {
        var filter = BuildFilter(status, searchTerm);
        return await ctx.ContactMessages.CountDocumentsAsync(filter, cancellationToken: ct);
    }

    public async Task<List<ContactMessage>> ListAsync(MessageStatus? status, string? searchTerm, int page, int pageSize, CancellationToken ct = default)
    {
        var filter = BuildFilter(status, searchTerm);
        return await ctx.ContactMessages.Find(filter)
            .SortByDescending(x => x.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync(ct);
    }

    private FilterDefinition<ContactMessage> BuildFilter(MessageStatus? status, string? searchTerm)
    {
        var builder = Builders<ContactMessage>.Filter;
        var filter = builder.Empty;

        if (status.HasValue)
        {
            filter &= builder.Eq(x => x.Status, status.Value);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            // Case-insensitive search on Name, Email, Subject
            var regex = new BsonRegularExpression(Regex.Escape(searchTerm), "i");
            var searchFilter = builder.Or(
                builder.Regex(x => x.Name, regex),
                builder.Regex(x => x.Email, regex),
                builder.Regex(x => x.Subject, regex)
            );
            filter &= searchFilter;
        }

        return filter;
    }
}