namespace BE_Portfolio.DTOs.Blog;

public record BlogResponseDto(
    string Id,
    string Title,
    string Slug,
    string Summary,
    string Content,
    string FeaturedImage,
    string CategoryId,
    List<string> Tags,
    bool Published,
    DateTime? PublishedAt,
    int ViewCount,
    DateTime CreatedAt,
    DateTime UpdateDate
);