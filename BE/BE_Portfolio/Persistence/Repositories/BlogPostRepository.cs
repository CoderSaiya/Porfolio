using BE_Portfolio.DTOs.Blog;
using BE_Portfolio.Models.Documents;
using BE_Portfolio.Persistence.Data;
using BE_Portfolio.Persistence.Repositories.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BE_Portfolio.Persistence.Repositories;

public class BlogPostRepository : IBlogPostRepository
{
    private readonly IMongoCollection<BlogPost> _collection;

    public BlogPostRepository(IMongoDbContext context)
    {
        _collection = context.BlogPosts;
    }

    public async Task<List<BlogPost>> GetAllAsync(BlogFilterDTO filter, CancellationToken ct = default)
    {
        var builder = Builders<BlogPost>.Filter;
        var filters = new List<FilterDefinition<BlogPost>>();

        // Filter by published status
        if (filter.Published.HasValue)
        {
            filters.Add(builder.Eq(x => x.Published, filter.Published.Value));
        }

        // Filter by category
        if (!string.IsNullOrEmpty(filter.CategoryId) && ObjectId.TryParse(filter.CategoryId, out var categoryId))
        {
            filters.Add(builder.Eq(x => x.CategoryId, categoryId));
        }

        // Filter by tags
        if (filter.Tags != null && filter.Tags.Any())
        {
            filters.Add(builder.AnyIn(x => x.Tags, filter.Tags));
        }

        // Search in title and summary
        if (!string.IsNullOrEmpty(filter.Search))
        {
            var searchFilter = builder.Or(
                builder.Regex(x => x.Title, new BsonRegularExpression(filter.Search, "i")),
                builder.Regex(x => x.Summary, new BsonRegularExpression(filter.Search, "i"))
            );
            filters.Add(searchFilter);
        }

        var finalFilter = filters.Any() ? builder.And(filters) : builder.Empty;

        return await _collection
            .Find(finalFilter)
            .Sort(Builders<BlogPost>.Sort.Descending(x => x.CreatedAt))
            .Skip((filter.Page - 1) * filter.PageSize)
            .Limit(filter.PageSize)
            .ToListAsync(ct);
    }

    public async Task<long> CountAsync(BlogFilterDTO filter, CancellationToken ct = default)
    {
        var builder = Builders<BlogPost>.Filter;
        var filters = new List<FilterDefinition<BlogPost>>();

        if (filter.Published.HasValue)
        {
            filters.Add(builder.Eq(x => x.Published, filter.Published.Value));
        }

        if (!string.IsNullOrEmpty(filter.CategoryId) && ObjectId.TryParse(filter.CategoryId, out var categoryId))
        {
            filters.Add(builder.Eq(x => x.CategoryId, categoryId));
        }

        if (filter.Tags != null && filter.Tags.Any())
        {
            filters.Add(builder.AnyIn(x => x.Tags, filter.Tags));
        }

        if (!string.IsNullOrEmpty(filter.Search))
        {
            var searchFilter = builder.Or(
                builder.Regex(x => x.Title, new BsonRegularExpression(filter.Search, "i")),
                builder.Regex(x => x.Summary, new BsonRegularExpression(filter.Search, "i"))
            );
            filters.Add(searchFilter);
        }

        var finalFilter = filters.Any() ? builder.And(filters) : builder.Empty;

        return await _collection.CountDocumentsAsync(finalFilter, cancellationToken: ct);
    }

    public async Task<BlogPost?> GetByIdAsync(string id, CancellationToken ct = default)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return null;

        return await _collection
            .Find(x => x.Id == objectId)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<BlogPost?> GetBySlugAsync(string slug, CancellationToken ct = default)
    {
        return await _collection
            .Find(x => x.Slug == slug)
            .FirstOrDefaultAsync(ct);
    }

    public async Task CreateAsync(BlogPost post, CancellationToken ct = default)
    {
        post.CreatedAt = DateTime.UtcNow;
        post.UpdateDate = DateTime.UtcNow;

        if (post.Published && !post.PublishedAt.HasValue)
        {
            post.PublishedAt = DateTime.UtcNow;
        }

        await _collection.InsertOneAsync(post, cancellationToken: ct);
    }

    public async Task UpdateAsync(BlogPost post, CancellationToken ct = default)
    {
        post.UpdateDate = DateTime.UtcNow;

        // Set PublishedAt when changing from draft to published
        if (post.Published && !post.PublishedAt.HasValue)
        {
            post.PublishedAt = DateTime.UtcNow;
        }
        // Clear PublishedAt when unpublishing
        else if (!post.Published && post.PublishedAt.HasValue)
        {
            post.PublishedAt = null;
        }

        await _collection.ReplaceOneAsync(x => x.Id == post.Id, post, cancellationToken: ct);
    }

    public async Task DeleteAsync(string id, CancellationToken ct = default)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return;

        await _collection.DeleteOneAsync(x => x.Id == objectId, ct);
    }

    public async Task IncrementViewCountAsync(string id, CancellationToken ct = default)
    {
        if (!ObjectId.TryParse(id, out var objectId))
            return;

        var update = Builders<BlogPost>.Update.Inc(x => x.ViewCount, 1);
        await _collection.UpdateOneAsync(x => x.Id == objectId, update, cancellationToken: ct);
    }

    public async Task<long> GetTotalViewsAsync(CancellationToken ct = default)
    {
        var result = await _collection.Aggregate()
            .Group(b => 1, g => new { TotalViews = g.Sum(x => x.ViewCount) })
            .FirstOrDefaultAsync(ct);

        return result?.TotalViews ?? 0;
    }
}
