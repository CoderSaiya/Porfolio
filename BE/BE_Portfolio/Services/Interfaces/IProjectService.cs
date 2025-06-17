using BE_Portfolio.DTOs;
using BE_Portfolio.Models.Documents;

namespace BE_Portfolio.Services.Interfaces
{
    public interface IProjectService
    {
        Task<ProjectsResponse?> GetByIdAsync(string id);
        Task<IEnumerable<ProjectsResponse>> GetAllAsync();
        Task<Project> CreateAsync(CreateProjectDto dto);
        Task DeleteAsync(string id);
    }
}
