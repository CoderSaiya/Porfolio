using System.ComponentModel.DataAnnotations.Schema;

namespace BE_Portfolio.Models;

[Table("Skills")]
public partial class Skill
{
    public int Id { get; set; }

    public string NameSkill { get; set; } = null!;

    public int Level { get; set; }
}
