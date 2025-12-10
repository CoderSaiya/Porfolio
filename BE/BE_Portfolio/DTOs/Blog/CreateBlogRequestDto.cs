namespace BE_Portfolio.DTOs.Blog;

public record CreateBlogRequestDto
{
    public string Title { get; init; } = null!;
    public string Slug { get; init; } = null!;
    public string Summary { get; init; } = null!;
    public string Content { get; init; } = null!;
    public string? CategoryId { get; init; }
    public List<string> Tags { get; init; } = new();
    public bool Published { get; init; } = false;
}
