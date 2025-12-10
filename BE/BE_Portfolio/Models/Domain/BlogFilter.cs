namespace BE_Portfolio.Models.Domain;

public record BlogFilter
{
    public string? CategoryId { get; init; }
    public List<string>? Tags { get; init; }
    public string? Search { get; init; }
    public bool? Published { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
