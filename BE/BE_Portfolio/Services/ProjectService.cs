using BE_Portfolio.DTOs;
using BE_Portfolio.Repositories.Interfaces;
using BE_Portfolio.Services.Interfaces;

namespace BE_Portfolio.Services
{
    public class ProjectServiceService(IProjectRepository projectRepository) : IProjectService
    {
        public async Task<IEnumerable<ProjectsWithTag>> GetProjectsWithTags()
        {
            var projects = await projectRepository.GetProjects();
            var projectsWithTags = projects.Select(p => new ProjectsWithTag{
                Id = p.Id,
                Title = p.Title,
                Platform = p.Platform,
                Position = p.Position,
                NumOfMember = p.NumOfMember,
                Description = p.Description ?? null,
                ImageUrl = p.ImageUrl,
                Tags = p.ProjectTags.Select(pt => pt.Tag).ToList()
                });

            return projectsWithTags;
        }
    }
}
