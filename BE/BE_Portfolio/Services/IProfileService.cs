using BE_Portfolio.Models.Documents;

namespace BE_Portfolio.Services;

public interface IProfileService
{
    Task<Profile?> GetProfileAsync(CancellationToken ct = default);
}
