using BE_Portfolio.Models.Domain;
using BE_Portfolio.Models.Documents;
using BE_Portfolio.Persistence.Data;
using BE_Portfolio.Persistence.Repositories.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BE_Portfolio.Persistence.Repositories;

public class ProjectRepository(IMongoDbContext ctx) : IProjectRepository
{
    private readonly IMongoCollection<Project> _projects = ctx.Projects;

    public async Task<List<Project>> GetAllAsync(ProjectFilter filter, CancellationToken ct = default)
    {
        var dbFilter = filter.HighlightOnly == true
            ? Builders<Project>.Filter.Eq(x => x.Highlight, true)
            : Builders<Project>.Filter.Empty;

        IFindFluent<Project, Project> find = _projects.Find(dbFilter).SortByDescending(x => x.CreatedAt);

        if (filter.Limit is > 0) find = find.Limit(filter.Limit);
        
        return await find.ToListAsync(ct);
    }

    public async Task<Project?> GetBySlugAsync(string slug, CancellationToken ct = default) =>
        await _projects.Find(x => x.Slug == slug).FirstOrDefaultAsync(ct);

    public async Task<Project?> GetByIdAsync(ObjectId id, CancellationToken ct = default)
    {
        var filter = Builders<Project>.Filter.Eq(p => p.Id, id);
        return await _projects.Find(filter).FirstOrDefaultAsync(ct);
    }

    public async Task CreateAsync(Project doc, CancellationToken ct = default)
    {
        if (doc.Id == default) doc.Id = ObjectId.GenerateNewId();
        await _projects.InsertOneAsync(doc, cancellationToken: ct);
    }

    public async Task InsertAsync(Project doc, CancellationToken ct = default)
    {
        if (doc.Id == default) doc.Id = ObjectId.GenerateNewId();
        if (string.IsNullOrWhiteSpace(doc.Slug))
            throw new ArgumentException("Slug is required.", nameof(doc));
        await _projects.InsertOneAsync(doc, cancellationToken: ct);
    }

    public async Task UpdateAsync(Project doc, CancellationToken ct = default)
    {
        var filter = Builders<Project>.Filter.Eq(p => p.Id, doc.Id);
        await _projects.ReplaceOneAsync(filter, doc, cancellationToken: ct);
    }

    public async Task DeleteAsync(ObjectId id, CancellationToken ct = default)
    {
        var filter = Builders<Project>.Filter.Eq(p => p.Id, id);
        await _projects.DeleteOneAsync(filter, cancellationToken: ct);
    }

    public async Task DeleteAsync(string slug, CancellationToken ct = default)
    {
        var filter = Builders<Project>.Filter.Eq(p => p.Slug, slug);
        await _projects.DeleteOneAsync(filter, cancellationToken: ct);
    }

    public async Task<long> CountAsync(ProjectFilter? filter = null, CancellationToken ct = default)
    {
        var dbFilter = Builders<Project>.Filter.Empty;
        if (filter?.HighlightOnly == true)
        {
            dbFilter = Builders<Project>.Filter.Eq(x => x.Highlight, true);
        }
        return await _projects.CountDocumentsAsync(dbFilter, cancellationToken: ct);
    }
}