using BE_Portfolio.Models.Documents;
using BE_Portfolio.DTOs.Blog;

namespace BE_Portfolio.Persistence.Repositories.Interfaces;

public interface IBlogPostRepository
{
    Task<List<BlogPost>> GetAllAsync(BlogFilterDTO filter, CancellationToken ct = default);
    Task<long> CountAsync(BlogFilterDTO filter, CancellationToken ct = default);
    Task<BlogPost?> GetByIdAsync(string id, CancellationToken ct = default);
    Task<BlogPost?> GetBySlugAsync(string slug, CancellationToken ct = default);
    Task CreateAsync(BlogPost post, CancellationToken ct = default);
    Task UpdateAsync(BlogPost post, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
    Task IncrementViewCountAsync(string id, CancellationToken ct = default);
}
