using BE_Portfolio.Models.Commons;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BE_Portfolio.Models.Documents;

[BsonIgnoreExtraElements]
public class Comment : BaseDocument
{
    [BsonElement("blogPostId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string BlogPostId { get; set; } = null!;

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string UserId { get; set; } = null!;

    [BsonElement("content")]
    public string Content { get; set; } = null!;

    [BsonElement("parentId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? ParentId { get; set; } // Null for top-level comments

    [BsonElement("likes")]
    public List<string> Likes { get; set; } = new(); // List of UserIds who liked

    [BsonElement("isDeleted")]
    public bool IsDeleted { get; set; } = false;
}
