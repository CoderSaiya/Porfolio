using BE_Portfolio.Models.Documents;

namespace BE_Portfolio.Persistence.Repositories.Interfaces;

public interface ISkillRepository
{
    Task<List<SkillCategory>> GetAllAsync(CancellationToken ct = default);
}