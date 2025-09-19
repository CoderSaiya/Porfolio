using BE_Portfolio.Models.Documents;
using BE_Portfolio.Persistence.Data;
using BE_Portfolio.Persistence.Repositories.Interfaces;
using MongoDB.Driver;

namespace BE_Portfolio.Persistence.Repositories;

public class ProfileRepository(IMongoDbContext ctx) : IProfileRepository
{
    public async Task<Profile?> GetAsync(CancellationToken ct = default)
        => await ctx.Profiles
            .Find(_ => true)
            .FirstOrDefaultAsync(ct);

    public async Task UpsertAsync(Profile doc, CancellationToken ct = default) 
        => await ctx.Profiles.ReplaceOneAsync(
            Builders<Profile>.Filter.Eq(x => x.Id, doc.Id),
            doc,
            new ReplaceOptions
            {
                IsUpsert = true
            },
            ct);
}