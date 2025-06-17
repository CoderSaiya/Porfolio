using System.ComponentModel.DataAnnotations.Schema;

namespace BE_Portfolio.Models;

[Table("Skills")]
public class Skill : BaseEntity
{
    public Guid SkillCategoryId { get; set; }
    [ForeignKey("SkillCategoryId")]
    public virtual SkillCategory? SkillCategory { get; set; }
    
    public string Name { get; set; } = null!;
}
