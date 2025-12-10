namespace BE_Portfolio.DTOs.Contact;

public record CreateContactRequestDto(
    string Name,
    string Email,
    string Subject,
    string Message
    );