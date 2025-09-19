using BE_Portfolio.Models.Commons;
using BE_Portfolio.Models.ValueObjects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BE_Portfolio.Models.Documents;

[BsonIgnoreExtraElements]
public class Image : BaseDocument
{
    [BsonElement("ownerType")]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public ImageOwnerType OwnerType { get; set; }

    [BsonElement("ownerId")]
    public ObjectId OwnerId { get; set; } 

    [BsonElement("variant")]
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public ImageVariant Variant { get; set; } = ImageVariant.Full;

    [BsonElement("mime")]
    public string MimeType { get; set; } = "image/webp";

    // Lưu NHỊ PHÂN để tối ưu, trả base64 khi response
    [BsonElement("data")] 
    public byte[] Data { get; set; } = [];

    [BsonElement("w")] 
    public int? Width { get; set; }
    [BsonElement("h")] 
    public int? Height { get; set; }

}