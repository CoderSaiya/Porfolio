namespace BE_Portfolio.DTOs.Comment;

public class CreateCommentRequestDTO
{
    public string BlogPostId { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string? ParentId { get; set; }
}

public class CommentResponseDTO
{
    public string Id { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string UserId { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public string? UserAvatar { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ParentId { get; set; }
    public List<CommentResponseDTO> Replies { get; set; } = new();
    public int LikesCount { get; set; }
    public bool IsLikedByCurrentUser { get; set; }
}
