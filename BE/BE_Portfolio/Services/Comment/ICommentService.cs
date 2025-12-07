using BE_Portfolio.DTOs.Comment;

namespace BE_Portfolio.Services.Comment;

public interface ICommentService
{
    Task<List<CommentResponseDTO>> GetCommentsByBlogIdAsync(string blogId, string? currentUserId, CancellationToken ct = default);
    Task<CommentResponseDTO?> AddCommentAsync(string userId, CreateCommentRequestDTO request, CancellationToken ct = default);
    Task<bool> DeleteCommentAsync(string commentId, string userId, bool isAdmin, CancellationToken ct = default);
}
