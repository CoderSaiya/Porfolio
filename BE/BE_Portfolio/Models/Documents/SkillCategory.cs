using BE_Portfolio.Models.Commons;
using MongoDB.Bson.Serialization.Attributes;

namespace BE_Portfolio.Models.Documents;

[BsonIgnoreExtraElements]
public class SkillCategory : BaseDocument
{
    [BsonElement("title")]
    public string Title { get; set; } = null!;
    [BsonElement("icon")]
    public string? Icon { get; set; } // "Server","Cloud"...
    [BsonElement("color")]
    public string? Color { get; set; } // "Server","Cloud"...
    [BsonElement("order")]
    public int Order { get; set; } = 0;

    [BsonElement("skills")]
    public List<SkillItem> Skills { get; set; } = new();
}