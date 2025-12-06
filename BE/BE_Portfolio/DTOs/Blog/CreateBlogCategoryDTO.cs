namespace BE_Portfolio.DTOs.Blog;

public record CreateBlogCategoryDTO
{
    public string Name { get; init; } = null!;
    public string Slug { get; init; } = null!;
    public string? Description { get; init; }
    public int Order { get; init; } = 0;
}
