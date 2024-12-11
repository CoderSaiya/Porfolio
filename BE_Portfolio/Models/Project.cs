using System;
using System.Collections.Generic;

namespace BE_Portfolio.Models;

public partial class Project
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string Platform { get; set; } = null!;

    public string Position { get; set; } = null!;

    public int NumOfMember { get; set; }

    public string? Description { get; set; }

    public string ImageUrl { get; set; } = null!;
}
