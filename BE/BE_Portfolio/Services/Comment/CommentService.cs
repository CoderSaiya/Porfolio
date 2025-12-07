using BE_Portfolio.DTOs.Comment;
using BE_Portfolio.Models.Documents;
using BE_Portfolio.Persistence.Repositories;

namespace BE_Portfolio.Services.Comment;

public class CommentService(ICommentRepository commentRepo, IUserRepository userRepo) : ICommentService
{
    public async Task<List<CommentResponseDTO>> GetCommentsByBlogIdAsync(string blogId, string? currentUserId, CancellationToken ct = default)
    {
        // Get all comments for the blog post
        var comments = await commentRepo.GetByBlogIdAsync(blogId, ct);

        // Get all distinct UserIds to fetch user details
        var userIds = comments.Select(c => c.UserId).Distinct().ToList();
        
        var userDict = new Dictionary<string, User>();
        foreach (var uid in userIds)
        {
            var u = await userRepo.GetByIdAsync(uid, ct);
            if (u != null) userDict[uid] = u;
        }

        // Map to DTOs (flat list first)
        var dtos = comments.Select(c =>
        {
            var user = userDict.GetValueOrDefault(c.UserId);
            return new CommentResponseDTO
            {
                Id = c.Id.ToString(),
                Content = c.Content,
                UserId = c.UserId,
                UserName = user?.FullName ?? user?.Username ?? "Unknown",
                UserAvatar = user?.AvatarUrl,
                CreatedAt = c.CreatedAt,
                ParentId = c.ParentId,
                LikesCount = c.Likes.Count,
                IsLikedByCurrentUser = currentUserId != null && c.Likes.Contains(currentUserId)
            };
        }).ToList();

        // Build Tree Structure
        var lookup = dtos.ToDictionary(c => c.Id);
        var rootComments = new List<CommentResponseDTO>();

        foreach (var dto in dtos)
        {
            if (string.IsNullOrEmpty(dto.ParentId))
            {
                rootComments.Add(dto);
            }
            else
            {
                if (lookup.TryGetValue(dto.ParentId, out var parent))
                {
                    parent.Replies.Add(dto);
                }
                // Else: orphan comment (parent deleted?), maybe add to root or ignore. Ignoring for now.
            }
        }

        // Sort replies by date
        foreach (var dto in dtos)
        {
            dto.Replies = dto.Replies.OrderBy(r => r.CreatedAt).ToList();
        }

        return rootComments.OrderByDescending(c => c.CreatedAt).ToList();
    }

    public async Task<CommentResponseDTO?> AddCommentAsync(string userId, CreateCommentRequestDTO request, CancellationToken ct = default)
    {
        var newComment = new Models.Documents.Comment
        {
            BlogPostId = request.BlogPostId,
            UserId = userId,
            Content = request.Content,
            ParentId = request.ParentId,
            CreatedAt = DateTime.UtcNow,
            UpdateDate = DateTime.UtcNow
        };

        await commentRepo.CreateAsync(newComment, ct);

        // Fetch user to return complete DTO
        var user = await userRepo.GetByIdAsync(userId, ct);

        return new CommentResponseDTO
        {
            Id = newComment.Id.ToString(),
            Content = newComment.Content,
            UserId = newComment.UserId,
            UserName = user?.FullName ?? user?.Username ?? "Unknown",
            UserAvatar = user?.AvatarUrl,
            CreatedAt = newComment.CreatedAt,
            ParentId = newComment.ParentId,
            LikesCount = 0,
            IsLikedByCurrentUser = false
        };
    }

    public async Task<bool> DeleteCommentAsync(string commentId, string userId, bool isAdmin, CancellationToken ct = default)
    {
        var comment = await commentRepo.GetByIdAsync(commentId, ct);
        if (comment == null) return false;

        if (!isAdmin && comment.UserId != userId) return false; // Not owner and not admin

        // Soft delete
        comment.IsDeleted = true;
        await commentRepo.UpdateAsync(comment, ct);
        return true;
    }
}
