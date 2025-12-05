using BE_Portfolio.Models.Documents;
using MongoDB.Bson;

namespace BE_Portfolio.Persistence.Repositories.Interfaces;

public interface ISkillRepository
{
    Task<List<SkillCategory>> GetAllAsync(CancellationToken ct = default);
    Task<SkillCategory?> GetCategoryByIdAsync(ObjectId id, CancellationToken ct = default);
    Task CreateCategoryAsync(SkillCategory category, CancellationToken ct = default);
    Task UpdateCategoryAsync(SkillCategory category, CancellationToken ct = default);
    Task DeleteCategoryAsync(ObjectId id, CancellationToken ct = default);
    Task UpdateCategoryOrderAsync(ObjectId id, int order, CancellationToken ct = default);
}