using BE_Portfolio.Models.Documents;
using MongoDB.Driver;

namespace BE_Portfolio.Repositories.Interfaces;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(string id);
    Task<IEnumerable<Project>> GetAllAsync();
    Task CreateAsync(Project project);
    Task UpdateAsync(Project project);
    Task DeleteAsync(string id);
}