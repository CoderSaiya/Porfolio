using BE_Portfolio.Data;
using BE_Portfolio.DTOs;
using BE_Portfolio.Models;
using BE_Portfolio.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BE_Portfolio.Services
{
    public class ProjectService : IProject
    {
        private readonly PortfolioDbContext _context;

        public ProjectService(PortfolioDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProjectsWithTag>?> GetProjects()
        {
            var projectsWithTags = await _context.Set<Project>()
           .Include(p => p.ProjectTags)
           .Select(p => new ProjectsWithTag
           {
               Id = p.Id,
               Title = p.Title,
               Platform = p.Platform,
               Position = p.Position,
               NumOfMember = p.NumOfMember,
               Description = p.Description,
               ImageUrl = p.ImageUrl,
               Tags = p.ProjectTags.Select(pt => pt.Tag).ToList()
           })
           .ToListAsync();

            if (!projectsWithTags.Any())
            {
                return null;
            }

            return projectsWithTags;
        }
    }
}
