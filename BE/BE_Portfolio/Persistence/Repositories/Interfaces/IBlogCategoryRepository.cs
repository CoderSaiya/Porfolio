using BE_Portfolio.Models.Documents;

namespace BE_Portfolio.Persistence.Repositories.Interfaces;

public interface IBlogCategoryRepository
{
    Task<List<BlogCategory>> GetAllAsync(CancellationToken ct = default);
    Task<BlogCategory?> GetByIdAsync(string id, CancellationToken ct = default);
    Task CreateAsync(BlogCategory category, CancellationToken ct = default);
    Task UpdateAsync(BlogCategory category, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}
