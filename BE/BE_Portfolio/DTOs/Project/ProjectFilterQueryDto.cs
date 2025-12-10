namespace BE_Portfolio.DTOs.Project;

public record ProjectFilterQueryDto
{
    public bool? HighlightOnly { get; init; }
    public int? Limit { get; init; }
}
