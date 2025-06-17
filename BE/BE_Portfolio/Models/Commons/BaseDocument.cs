using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;

namespace BE_Portfolio.Models.Commons;

public class BaseDocument
{
    [Key]
    public ObjectId Id { get; set; }
    public DateTime CreateDate { get; set; } = DateTime.Now;
    public DateTime UpdateDate { get; set; } = DateTime.Now;
}