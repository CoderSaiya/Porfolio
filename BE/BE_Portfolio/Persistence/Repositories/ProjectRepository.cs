using BE_Portfolio.Models.Documents;
using BE_Portfolio.Persistence.Data;
using BE_Portfolio.Persistence.Repositories.Interfaces;
using MongoDB.Driver;

namespace BE_Portfolio.Persistence.Repositories;

public class ProjectRepository(IMongoDbContext ctx) : IProjectRepository
{
    public async Task<List<Project>> GetAllAsync(bool? highlightOnly, int? limit, CancellationToken ct = default)
    {
        var filter = highlightOnly == true
            ? Builders<Project>.Filter.Eq(x => x.Highlight, true)
            : Builders<Project>.Filter.Empty;

        IFindFluent<Project, Project> find = ctx.Projects.Find(filter).SortByDescending(x => x.CreatedAt);

        if (limit is > 0) find = find.Limit(limit);
        
        return await find.ToListAsync(ct);
    }

    public async Task<Project?> GetBySlugAsync(string slug, CancellationToken ct = default) =>
        await ctx.Projects.Find(x => x.Slug == slug).FirstOrDefaultAsync(ct);

    public Task InsertAsync(Project doc, CancellationToken ct = default)
    {
        if (doc.Id == default) doc.Id = MongoDB.Bson.ObjectId.GenerateNewId();
        if (string.IsNullOrWhiteSpace(doc.Slug))
            throw new ArgumentException("Slug is required.", nameof(doc));
        return ctx.Projects.InsertOneAsync(doc, cancellationToken: ct);
    }

    public Task UpdateAsync(Project doc, CancellationToken ct = default) =>
        ctx.Projects.ReplaceOneAsync(x => x.Id == doc.Id, doc, cancellationToken: ct);

    public Task DeleteAsync(string slug, CancellationToken ct = default) =>
        ctx.Projects.DeleteOneAsync(x => x.Slug == slug, ct);
}