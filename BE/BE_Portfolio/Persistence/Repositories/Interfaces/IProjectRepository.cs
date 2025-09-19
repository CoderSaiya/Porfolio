using BE_Portfolio.Models.Documents;

namespace BE_Portfolio.Persistence.Repositories.Interfaces;

public interface IProjectRepository
{
    Task<List<Project>> GetAllAsync(bool? highlightOnly, int? limit, CancellationToken ct = default);
    Task<Project?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task InsertAsync(Project doc, CancellationToken ct = default);
    Task UpdateAsync(Project doc, CancellationToken ct = default);
    Task DeleteAsync(string slug, CancellationToken ct = default);
}