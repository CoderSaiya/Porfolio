using BE_Portfolio.Models.ValueObjects;

namespace BE_Portfolio.DTOs.Profile;

public record UpdateProfileRequestDto
{
    public string? FullName { get; init; }
    public string? Headline { get; init; }
    public string? Location { get; init; }
    public string? About { get; init; }
    public int? YearsExperience { get; init; }
    public int? ProjectsCompleted { get; init; }
    public int? Coffees { get; init; }
    public List<SocialLinkDTO>? SocialLinks { get; init; }
}

public record SocialLinkDTO
{
    public SocialPlatform Platform { get; init; }
    public string Url { get; init; } = null!;
    public int Order { get; init; }
}
