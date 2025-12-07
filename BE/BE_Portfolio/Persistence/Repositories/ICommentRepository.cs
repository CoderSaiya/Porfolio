using BE_Portfolio.Models.Documents;

namespace BE_Portfolio.Persistence.Repositories;

public interface ICommentRepository
{
    Task<List<Comment>> GetByBlogIdAsync(string blogId, CancellationToken ct = default);
    Task<Comment?> GetByIdAsync(string id, CancellationToken ct = default);
    Task CreateAsync(Comment comment, CancellationToken ct = default);
    Task UpdateAsync(Comment comment, CancellationToken ct = default);
    Task DeleteAsync(string id, CancellationToken ct = default);
}
