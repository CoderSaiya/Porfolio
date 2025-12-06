using BE_Portfolio.Models.Documents;
using BE_Portfolio.Persistence.Data;
using BE_Portfolio.Persistence.Repositories.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BE_Portfolio.Persistence.Repositories;

public class BlogCategoryRepository : IBlogCategoryRepository
{
    private readonly IMongoCollection<BlogCategory> _collection;

    public BlogCategoryRepository(IMongoDbContext context)
    {
        _collection = context.BlogCategories;
    }

    public async Task<List<BlogCategory>> GetAllAsync(CancellationToken ct = default)
    {
        return await _collection
            .Find(_ => true)
            .Sort(Builders<BlogCategory>.Sort.Ascending(x => x.Order))
            .ToListAsync(ct);
    }

    public async Task<BlogCategory?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return null;

        return await _collection
            .Find(x => x.Id == objectId)
            .FirstOrDefaultAsync(ct);
    }

    public async Task CreateAsync(BlogCategory category, CancellationToken ct = default)
    {
        category.CreatedAt = DateTime.UtcNow;
        category.UpdateDate = DateTime.UtcNow;
        await _collection.InsertOneAsync(category, cancellationToken: ct);
    }

    public async Task UpdateAsync(BlogCategory category, CancellationToken ct = default)
    {
        category.UpdateDate = DateTime.UtcNow;
        await _collection.ReplaceOneAsync(x => x.Id == category.Id, category, cancellationToken: ct);
    }

    public async Task DeleteAsync(string id, CancellationToken ct = default)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return;

        await _collection.DeleteOneAsync(x => x.Id == objectId, ct);
    }
}
