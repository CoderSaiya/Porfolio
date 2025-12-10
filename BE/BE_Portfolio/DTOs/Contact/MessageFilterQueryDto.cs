using BE_Portfolio.Models.ValueObjects;

namespace BE_Portfolio.DTOs.Contact;

public record MessageFilterQueryDto
{
    public MessageStatus? Status { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? SearchTerm { get; init; }
}
