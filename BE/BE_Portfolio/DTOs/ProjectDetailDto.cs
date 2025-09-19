namespace BE_Portfolio.DTOs;

public sealed record ProjectDetailDto(
    string Id,
    string Slug,
    string Title,
    string Description,
    bool Highlight,
    int? Duration,
    int? TeamSize,
    IReadOnlyList<string> Technologies,
    IReadOnlyList<string> Features,
    string? Github,
    string? Demo,
    string? Image,
    string? Thumb
);