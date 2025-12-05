namespace BE_Portfolio.DTOs.Admin;

public record UpdateProjectDTO
{
    public string? Title { get; init; }
    public string? Slug { get; init; }
    public string? Description { get; init; }
    public bool? Highlight { get; init; }
    public int? Duration { get; init; }
    public int? TeamSize { get; init; }
    public string? Github { get; init; }
    public string? Demo { get; init; }
    public List<string>? Technologies { get; init; }
    public List<string>? Features { get; init; }
}
