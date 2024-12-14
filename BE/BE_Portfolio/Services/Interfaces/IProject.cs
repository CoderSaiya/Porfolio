using BE_Portfolio.DTOs;

namespace BE_Portfolio.Services.Interfaces
{
    public interface IProject
    {
        public Task<List<ProjectsWithTag>?> GetProjects();
    }
}
