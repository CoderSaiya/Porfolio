using BE_Portfolio.Data;
using BE_Portfolio.Models.Entities;
using BE_Portfolio.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BE_Portfolio.Repositories;

public class SkillRepository(PortfolioDbContext context) : ISkillRepository
{
    public async Task<SkillCategory> AddCategoryAsync(SkillCategory skillCategory)
    {
        await context.SkillCategories.AddAsync(skillCategory);
        return skillCategory;
    }

    public async Task<IEnumerable<SkillCategory>> GetSkillCategories()
    {
        return await context.SkillCategories
            .Include(sc => sc.Skills)
            .ToListAsync();
    }

    public async Task<SkillCategory?> GetCategoryByIdAsync(Guid id)
    {
        return await context.SkillCategories.FindAsync(id);
    }
}