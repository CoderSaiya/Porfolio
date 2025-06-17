using System.ComponentModel.DataAnnotations.Schema;
using BE_Portfolio.Models.Commons;

namespace BE_Portfolio.Models.Entities;

[Table("Skills")]
public class Skill : BaseEntity
{
    public Guid SkillCategoryId { get; set; }
    [ForeignKey("SkillCategoryId")]
    public virtual SkillCategory? SkillCategory { get; set; }
    
    public string Name { get; set; } = null!;
}
