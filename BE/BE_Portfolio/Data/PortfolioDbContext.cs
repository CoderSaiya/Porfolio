using BE_Portfolio.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BE_Portfolio.Data;

public partial class PortfolioDbContext : DbContext
{
    public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options)
        : base(options)
    {
    }
    public virtual DbSet<Skill> Skills { get; set; }
    public virtual DbSet<SkillCategory> SkillCategories { get; set; }
}
