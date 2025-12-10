namespace BE_Portfolio.DTOs.Contact;

public record BulkDeleteRequestDto
{
    public List<string> Ids { get; init; } = new();
}
