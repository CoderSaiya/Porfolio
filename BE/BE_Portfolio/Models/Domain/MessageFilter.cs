using BE_Portfolio.Models.ValueObjects;

namespace BE_Portfolio.Models.Domain;

public record MessageFilter
{
    public MessageStatus? Status { get; init; }
    public string? SearchTerm { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}
