using BE_Portfolio.Models.ValueObjects;

namespace BE_Portfolio.DTOs.Contact;

public record ContactMessageResponseDto(
    string Id,
    string Name,
    string Email,
    string Subject,
    string Message,
    MessageStatus Status,
    string? Ip,
    string? UserAgent,
    DateTime? ReadAt,
    DateTime CreatedAt
);
