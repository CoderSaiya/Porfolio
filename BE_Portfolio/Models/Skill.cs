using System;
using System.Collections.Generic;

namespace BE_Portfolio.Models;

public partial class Skill
{
    public int Id { get; set; }

    public string NameSkill { get; set; } = null!;

    public int Level { get; set; }
}
