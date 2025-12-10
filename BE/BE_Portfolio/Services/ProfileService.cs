using BE_Portfolio.Models.Documents;
using BE_Portfolio.Persistence.Repositories.Interfaces;

namespace BE_Portfolio.Services;

public class ProfileService(IProfileRepository repo) : IProfileService
{
    public Task<Profile?> GetProfileAsync(CancellationToken ct = default)
        => repo.GetAsync(ct);
}
