namespace BE_Portfolio.DTOs;

public record ContactCreateReq(
    string Name,
    string Email,
    string Subject,
    string Message
    );