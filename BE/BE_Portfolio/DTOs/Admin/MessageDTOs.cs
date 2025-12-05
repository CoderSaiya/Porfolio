using BE_Portfolio.Models.ValueObjects;

namespace BE_Portfolio.DTOs.Admin;

public record MessageFilterDTO
{
    public MessageStatus? Status { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? SearchTerm { get; init; }
}

public record UpdateMessageStatusDTO
{
    public MessageStatus Status { get; init; }
}

public record BulkDeleteDTO
{
    public List<string> Ids { get; init; } = new();
}
