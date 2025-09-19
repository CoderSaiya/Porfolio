using System.ComponentModel.DataAnnotations;
using BE_Portfolio.Models.Commons;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace BE_Portfolio.Models.Documents;

[BsonIgnoreExtraElements]
public class Profile : BaseDocument
{
    [BsonElement("fullName")]
    public string FullName { get; set; } = null!;
    [BsonElement("headline")]
    public string Headline { get; set; } = null!;
    [BsonElement("location")]
    public string Location { get; set; } = null!;
    [BsonElement("avatarUrl")]
    public string? AvatarUrl { get; set; }
    [BsonElement("about")]
    public string? About { get; set; }

    // Stats
    [BsonElement("yearsExperience")]
    public int YearsExperience { get; set; } = 0;
    [BsonElement("projectsCompleted")]
    public int ProjectsCompleted { get; set; } = 0;
    [BsonElement("coffees")]
    public int Coffees { get; set; } = 0;

    [BsonElement("social")]
    public List<SocialLink> SocialLinks { get; set; } = new();
}