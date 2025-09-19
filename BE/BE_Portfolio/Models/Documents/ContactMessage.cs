using BE_Portfolio.Models.Commons;
using BE_Portfolio.Models.ValueObjects;
using MongoDB.Bson.Serialization.Attributes;

namespace BE_Portfolio.Models.Documents;

[BsonIgnoreExtraElements]
public class ContactMessage : BaseDocument
{
    [BsonElement("name")] 
    public string Name { get; set; } = null!;
    [BsonElement("email")] 
    public string Email { get; set; } = null!;
    [BsonElement("subject")]
    public string Subject { get; set; } = null!;
    [BsonElement("message")]
    public string Message { get; set; } = null!;
    
    [BsonElement("status")]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public MessageStatus Status { get; set; } = MessageStatus.New;

    [BsonElement("ip")]
    public string? Ip { get; set; }
    [BsonElement("userAgent")]
    public string? UserAgent { get; set; }

    [BsonElement("readAt"), BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? ReadAt { get; set; }
}