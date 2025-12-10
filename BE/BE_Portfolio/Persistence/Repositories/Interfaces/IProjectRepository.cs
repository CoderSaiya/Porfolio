using BE_Portfolio.Models.Domain;
using BE_Portfolio.Models.Documents;
using MongoDB.Bson;

namespace BE_Portfolio.Persistence.Repositories.Interfaces;

public interface IProjectRepository
{
    Task<List<Project>> GetAllAsync(ProjectFilter filter, CancellationToken ct = default);
    Task<Project?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task<Project?> GetByIdAsync(ObjectId id, CancellationToken ct = default);
    Task CreateAsync(Project doc, CancellationToken ct = default);
    Task InsertAsync(Project doc, CancellationToken ct = default);
    Task UpdateAsync(Project doc, CancellationToken ct = default);
    Task DeleteAsync(ObjectId id, CancellationToken ct = default);
    Task DeleteAsync(string slug, CancellationToken ct = default);
    Task<long> CountAsync(ProjectFilter? filter = null, CancellationToken ct = default);
}