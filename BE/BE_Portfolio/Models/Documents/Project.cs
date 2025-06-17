using System.ComponentModel.DataAnnotations.Schema;
using BE_Portfolio.Models.Commons;
using MongoDB.Bson.Serialization.Attributes;

namespace BE_Portfolio.Models.Documents;

public class Project : BaseDocument
{
    [BsonElement("title")]
    public string Title { get; set; } = null!;

    [BsonElement("platform")]
    public string Platform { get; set; } = null!;

    [BsonElement("position")]
    public string Position { get; set; } = null!;

    [BsonElement("numOfMember")]
    public int NumOfMember { get; set; }

    [BsonElement("description")]
    public string? Description { get; set; }

    [BsonElement("link")]
    public string? Link { get; set; }

    [BsonElement("demo")]
    public string? Demo { get; set; }

    [BsonElement("imageData")]
    public byte[] ImageData { get; set; } = [];
    
    [BsonElement("tags")]
    public List<string> Tags { get; set; } = new();
}
