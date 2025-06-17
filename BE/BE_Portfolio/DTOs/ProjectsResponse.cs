using MongoDB.Bson;

namespace BE_Portfolio.DTOs
{
    public record ProjectsResponse
    (
        string Id,
        string Title,
        string Platform,
        string Position,
        int NumOfMember,
        string? Description,
        byte[] ImageData,
        string? Link,
        string? Demo,
        List<string> Tags
    );
}
