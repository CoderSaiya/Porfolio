using BE_Portfolio.Models;
using Microsoft.EntityFrameworkCore;

namespace BE_Portfolio.Data;

public partial class PortfolioDbContext : DbContext
{
    public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectTag> ProjectTags { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    
}
