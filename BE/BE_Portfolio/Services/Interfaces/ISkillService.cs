using BE_Portfolio.Models;

namespace BE_Portfolio.Services.Interfaces
{
    public interface ISkill
    {
        public Task<IEnumerable<Skill>> GetSkillsList();
    }
}
