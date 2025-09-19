using BE_Portfolio.Models.Documents;
using BE_Portfolio.Persistence.Data;
using BE_Portfolio.Persistence.Repositories.Interfaces;
using MongoDB.Driver;

namespace BE_Portfolio.Persistence.Repositories;

public class SkillRepository(IMongoDbContext ctx) : ISkillRepository
{
    public async Task<List<SkillCategory>> GetAllAsync(CancellationToken ct = default)
        => await ctx.SkillCategories
            .Find(_ => true)
            .SortBy(x => x.Order)
            .ToListAsync(ct);
}