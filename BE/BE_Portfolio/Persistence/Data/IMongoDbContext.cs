using BE_Portfolio.Models.Documents;
using MongoDB.Driver;
using BE_Portfolio.Models.Documents;
using MongoDB.Driver;

namespace BE_Portfolio.Persistence.Data;

public interface IMongoDbContext
{
    IMongoCollection<Profile> Profiles { get; }
    IMongoCollection<Project> Projects { get; }
    IMongoCollection<SkillCategory> SkillCategories { get; }
    IMongoCollection<ContactMessage> ContactMessages { get; }
    IMongoCollection<Image> Images { get; }
    IMongoCollection<User> Users { get; }

    Task EnsureIndexesAsync(CancellationToken ct = default);
    Task EnsureSeedAsync(CancellationToken ct = default);
}