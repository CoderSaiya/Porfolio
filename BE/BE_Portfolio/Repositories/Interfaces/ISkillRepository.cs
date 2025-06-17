using BE_Portfolio.Models.Entities;

namespace BE_Portfolio.Repositories.Interfaces;

public interface ISkillRepository
{
    public Task<SkillCategory> AddCategoryAsync(SkillCategory skillCategory);
    public Task<IEnumerable<SkillCategory>> GetSkillCategories();
    public Task<SkillCategory?> GetCategoryByIdAsync(Guid id);
}