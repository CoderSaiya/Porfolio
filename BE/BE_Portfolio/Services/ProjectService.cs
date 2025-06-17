using BE_Portfolio.DTOs;
using BE_Portfolio.Models.Documents;
using BE_Portfolio.Repositories.Interfaces;
using BE_Portfolio.Services.Interfaces;

namespace BE_Portfolio.Services
{
    public class ProjectService(IProjectRepository projectRepository) : IProjectService
    {
        public async Task<ProjectsResponse?> GetByIdAsync(string id)
        {
            var project = await projectRepository.GetByIdAsync(id);
            
            if (project == null)
                return null;

            return new ProjectsResponse(
                Id: project.Id.ToString(),
                Title: project.Title,
                Platform: project.Platform,
                Position: project.Position,
                NumOfMember: project.NumOfMember,
                Description: project.Description,
                ImageData: project.ImageData,
                Link: project.Link ?? null,
                Demo: project.Demo ?? null,
                Tags: project.Tags
            );
        }

        public async Task<IEnumerable<ProjectsResponse>> GetAllAsync()
        {
            var projects = await projectRepository.GetAllAsync();

            return projects.Select(p => new ProjectsResponse
            (
                Id: p.Id.ToString(),
                Title: p.Title,
                Platform: p.Platform,
                Position: p.Position,
                NumOfMember: p.NumOfMember,
                Description: p.Description,
                ImageData: p.ImageData,
                Link: p.Link ?? null,
                Demo: p.Demo ?? null,
                Tags: p.Tags
            ));
        }

        public async Task<Project> CreateAsync(CreateProjectDto dto)
        {
            var project = new Project
            {
                Title = dto.Title,
                Platform = dto.Platform,
                Position = dto.Position,
                NumOfMember = dto.NumOfMember,
                Description = dto.Description,
                Link = dto.Link,
                Demo = dto.Demo,
                Tags = dto.Tags,
                ImageData = await ReadFileAsync(dto.ImageFile)
            };
            
            await projectRepository.CreateAsync(project);
            return project;
        }

        public async Task DeleteAsync(string id) => await projectRepository.DeleteAsync(id);
        
        private static async Task<byte[]> ReadFileAsync(IFormFile file)
        {
            if (file.Length == 0) return [];
            using var ms = new System.IO.MemoryStream();
            await file.CopyToAsync(ms);
            return ms.ToArray();
        }
    }
}
