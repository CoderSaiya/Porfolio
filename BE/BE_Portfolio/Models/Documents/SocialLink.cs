using BE_Portfolio.Models.Commons;
using BE_Portfolio.Models.ValueObjects;
using MongoDB.Bson.Serialization.Attributes;

namespace BE_Portfolio.Models.Documents;

[BsonIgnoreExtraElements]
public class SocialLink : BaseDocument
{
    [BsonElement("platform")]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)] // lưu enum bằng tên
    public SocialPlatform Platform { get; set; }

    [BsonElement("url")]
    public string Url { get; set; } = null!;
    [BsonElement("order")]
    public int Order { get; set; } = 0;
}