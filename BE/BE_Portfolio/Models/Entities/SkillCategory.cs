using BE_Portfolio.Models.Commons;

namespace BE_Portfolio.Models.Entities;

public class SkillCategory : BaseEntity
{
    public string Title { get; set; } = null!;
    public string IconName { get; set; } = null!;
    public string Color { get; set; } = null!;
    
    public ICollection<Skill> Skills { get; set; } = new List<Skill>();
}