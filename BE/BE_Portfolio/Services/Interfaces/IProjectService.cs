using BE_Portfolio.DTOs;

namespace BE_Portfolio.Services.Interfaces
{
    public interface IProject
    {
        public Task<IEnumerable<ProjectsWithTag>> GetProjectsWithTags();
    }
}
