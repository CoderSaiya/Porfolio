using System;
using System.Collections.Generic;

namespace BE_Portfolio.Models;

public partial class ProjectTag
{
    public int? ProjectId { get; set; }

    public string Tag { get; set; } = null!;

    public virtual Project? Project { get; set; }
}
