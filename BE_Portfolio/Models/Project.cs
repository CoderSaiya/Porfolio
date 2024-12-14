﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BE_Portfolio.Models;

[Table("Projects")]
public partial class Project
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Platform { get; set; } = null!;

    public string Position { get; set; } = null!;

    public int NumOfMember { get; set; }

    public string? Description { get; set; }

    public string ImageUrl { get; set; } = null!;
    public virtual ICollection<ProjectTag> ProjectTags { get; set; } = new HashSet<ProjectTag>();
}
