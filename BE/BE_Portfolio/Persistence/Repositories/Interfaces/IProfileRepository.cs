using BE_Portfolio.Models.Documents;

namespace BE_Portfolio.Persistence.Repositories.Interfaces;

public interface IProfileRepository
{
    Task<Profile?> GetAsync(CancellationToken ct = default);
    Task UpsertAsync(Profile doc, CancellationToken ct = default);
    
    // Aliases for clarity/plan compliance
    Task<Profile?> GetProfileAsync(CancellationToken ct = default);
    Task UpdateProfileAsync(Profile doc, CancellationToken ct = default);
}