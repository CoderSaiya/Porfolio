using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BE_Portfolio.Models.Commons;

public class BaseDocument
{
    [Key]
    public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
    [BsonElement("createdAt"), BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [BsonElement("updatedAt"), BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime UpdateDate { get; set; } = DateTime.Now;
}