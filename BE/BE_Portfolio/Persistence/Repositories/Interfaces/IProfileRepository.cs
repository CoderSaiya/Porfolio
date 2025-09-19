using BE_Portfolio.Models.Documents;

namespace BE_Portfolio.Persistence.Repositories.Interfaces;

public interface IProfileRepository
{
    Task<Profile?> GetAsync(CancellationToken ct = default);
    Task UpsertAsync(Profile doc, CancellationToken ct = default);
}