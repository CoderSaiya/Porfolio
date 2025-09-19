using System.ComponentModel.DataAnnotations.Schema;
using BE_Portfolio.Models.Commons;
using MongoDB.Bson.Serialization.Attributes;

namespace BE_Portfolio.Models.Documents;

[BsonIgnoreExtraElements]
public class Project : BaseDocument
{
    [BsonElement("title")]
    public string Title { get; set; } = null!;
    [BsonElement("slug")]
    public string Slug { get; set; } = null!; // unique
    [BsonElement("description")]
    public string Description { get; set; } = null!;
    [BsonElement("highlight")]
    public bool Highlight { get; set; } = false;

    [BsonElement("durationMonths")]
    public int? Duration { get; set; }
    [BsonElement("teamSize")]
    public int? TeamSize { get; set; }

    [BsonElement("startedAt"), BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? StartedAt { get; set; }

    [BsonElement("endedAt"), BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
    public DateTime? EndedAt { get; set; }

    [BsonElement("repoUrl")]
    public string? Github { get; set; }
    [BsonElement("demoUrl")]
    public string? Demo { get; set; }
    [BsonElement("imageUrl")]
    public string? ImageUrl { get; set; }

    [BsonElement("technologies")]
    public List<string> Technologies { get; set; } = new();
    [BsonElement("features")]
    public List<string> Features { get; set; } = new();

    [BsonElement("order")]
    public int Order { get; set; } = 0;
}
