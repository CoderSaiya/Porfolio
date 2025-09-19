namespace BE_Portfolio.DTOs;

public sealed record ProjectListItemDto(
    string Id,
    string Slug,
    string Title,
    string Description,
    bool Highlight,
    int? Duration,
    int? TeamSize,
    IReadOnlyList<string> Technologies,
    IReadOnlyList<string> Features,
    string? Thumb,
    string? Github,
    string? Demo);