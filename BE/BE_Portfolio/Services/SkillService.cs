using BE_Portfolio.Models.Documents;
using BE_Portfolio.Persistence.Repositories.Interfaces;

namespace BE_Portfolio.Services;

public class SkillService(ISkillRepository repo) : ISkillService
{
    public async Task<IEnumerable<SkillCategory>> GetSkillsAsync(CancellationToken ct = default)
        => await repo.GetAllAsync(ct);
}
