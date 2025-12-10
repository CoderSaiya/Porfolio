namespace BE_Portfolio.DTOs.Blog;

public record UpdateBlogRequestDto
{
    public string? Title { get; init; }
    public string? Slug { get; init; }
    public string? Summary { get; init; }
    public string? Content { get; init; }
    public string? FeaturedImage { get; init; }
    public string? CategoryId { get; init; }
    public List<string>? Tags { get; init; }
    public bool? Published { get; init; }
}
