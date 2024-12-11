using System;
using System.Collections.Generic;
using BE_Portfolio.Models;
using Microsoft.EntityFrameworkCore;

namespace BE_Portfolio.Data;

public partial class PortfolioDbContext : DbContext
{
    public PortfolioDbContext()
    {
    }

    public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<ProjectTag> ProjectTags { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-1FAVEMH\\SQLEXPRESS;Initial Catalog=Portfolio;Integrated Security=True;trusted_connection=true;encrypt=false;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Projects__3214EC0765B3B8F0");

            entity.Property(e => e.ImageUrl).HasMaxLength(256);
            entity.Property(e => e.Platform).HasMaxLength(50);
            entity.Property(e => e.Position).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<ProjectTag>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Tag).HasMaxLength(100);

            entity.HasOne(d => d.Project).WithMany()
                .HasForeignKey(d => d.ProjectId)
                .HasConstraintName("FK__ProjectTa__Proje__52593CB8");
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Skills__3214EC07FC17361E");

            entity.HasIndex(e => e.NameSkill, "UQ__Skills__2B5CE523AFB6CDDE").IsUnique();

            entity.Property(e => e.NameSkill).HasMaxLength(100);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
