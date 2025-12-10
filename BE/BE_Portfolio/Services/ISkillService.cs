using BE_Portfolio.Models.Documents;

namespace BE_Portfolio.Services;

public interface ISkillService
{
    Task<IEnumerable<SkillCategory>> GetSkillsAsync(CancellationToken ct = default);
}
