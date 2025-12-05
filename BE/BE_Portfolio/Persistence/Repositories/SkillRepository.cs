using BE_Portfolio.Models.Documents;
using BE_Portfolio.Persistence.Data;
using BE_Portfolio.Persistence.Repositories.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

namespace BE_Portfolio.Persistence.Repositories;

public class SkillRepository(IMongoDbContext ctx) : ISkillRepository
{
    private readonly IMongoCollection<SkillCategory> _categories = ctx.SkillCategories;

    public async Task<List<SkillCategory>> GetAllAsync(CancellationToken ct = default)
    {
        return await _categories.Find(_ => true)
            .SortBy(x => x.Order)
            .ToListAsync(ct);
    }

    public async Task<SkillCategory?> GetCategoryByIdAsync(ObjectId id, CancellationToken ct = default)
    {
        return await _categories.Find(x => x.Id == id).FirstOrDefaultAsync(ct);
    }

    public async Task CreateCategoryAsync(SkillCategory category, CancellationToken ct = default)
    {
        if (category.Id == default) 
            category.Id = ObjectId.GenerateNewId();
        
        await _categories.InsertOneAsync(category, cancellationToken: ct);
    }

    public async Task UpdateCategoryAsync(SkillCategory category, CancellationToken ct = default)
    {
        var filter = Builders<SkillCategory>.Filter.Eq(x => x.Id, category.Id);
        await _categories.ReplaceOneAsync(filter, category, cancellationToken: ct);
    }

    public async Task DeleteCategoryAsync(ObjectId id, CancellationToken ct = default)
    {
        var filter = Builders<SkillCategory>.Filter.Eq(x => x.Id, id);
        await _categories.DeleteOneAsync(filter, cancellationToken: ct);
    }

    public async Task UpdateCategoryOrderAsync(ObjectId id, int order, CancellationToken ct = default)
    {
        var filter = Builders<SkillCategory>.Filter.Eq(x => x.Id, id);
        var update = Builders<SkillCategory>.Update.Set(x => x.Order, order);
        await _categories.UpdateOneAsync(filter, update, cancellationToken: ct);
    }
}