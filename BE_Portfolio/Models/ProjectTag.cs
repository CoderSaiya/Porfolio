using System.ComponentModel.DataAnnotations.Schema;

namespace BE_Portfolio.Models;

[Table("ProjectTags")]
public partial class ProjectTag
{
    public int Id { get; set; }

    public int ProjectId { get; set; }

    public string Tag { get; set; } = null!;

    public virtual Project? Project { get; set; }
}
