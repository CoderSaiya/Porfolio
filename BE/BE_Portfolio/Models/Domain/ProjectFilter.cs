namespace BE_Portfolio.Models.Domain;

public record ProjectFilter
{
    public bool? HighlightOnly { get; init; }
    public int? Limit { get; init; }
}
