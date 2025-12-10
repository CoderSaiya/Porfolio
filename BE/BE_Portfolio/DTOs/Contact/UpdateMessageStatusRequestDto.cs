using BE_Portfolio.Models.ValueObjects;

namespace BE_Portfolio.DTOs.Contact;

public record UpdateMessageStatusRequestDto
{
    public MessageStatus Status { get; init; }
}
