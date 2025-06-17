using BE_Portfolio.Data;
using BE_Portfolio.Models.Documents;
using BE_Portfolio.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace BE_Portfolio.Repositories;

public class ProjectRepository(MongoDbContext context) : IProjectRepository
{
    public async Task<Project?> GetByIdAsync(string id)
    {
        var filter = Builders<Project>.Filter.Eq(p => p.Id.ToString(), id);
        return await context.Projects.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Project>> GetAllAsync()
    {
        var filter = Builders<Project>.Filter.Empty;
        return await context.Projects.Find(filter).ToListAsync();
    }

    public async Task CreateAsync(Project project)
    {
        await context.Projects.InsertOneAsync(project);
    }

    public async Task UpdateAsync(Project project)
    {
        await context.Projects.ReplaceOneAsync(p => p.Id == project.Id, project);
    }

    public async Task DeleteAsync(string id)
    {
        await context.Projects.DeleteOneAsync(p => p.ToString() == id);
    }
}