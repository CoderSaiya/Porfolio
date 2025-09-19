﻿using MongoDB.Bson.Serialization.Attributes;

namespace BE_Portfolio.Models.Documents;

[BsonIgnoreExtraElements]
public class SkillItem
{
    [BsonElement("name")] 
    public string Name { get; set; } = null!;
    [BsonElement("level")] 
    public int Level { get; set; } = 0; // 0-100
    [BsonElement("order")] 
    public int Order { get; set; } = 0;
}