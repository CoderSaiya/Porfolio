using BE_Portfolio.Models.Commons;
using MongoDB.Bson.Serialization.Attributes;

namespace BE_Portfolio.Models.Documents;

[BsonIgnoreExtraElements]
public class BlogCategory : BaseDocument
{
    [BsonElement("name")]
    public string Name { get; set; } = null!;

    [BsonElement("slug")]
    public string Slug { get; set; } = null!;

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("order")]
    public int Order { get; set; } = 0;
}
