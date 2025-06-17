using BE_Portfolio.DTOs;
using BE_Portfolio.Models;

namespace BE_Portfolio.Services.Interfaces
{
    public interface ISkillService
    {
        public Task<SkillCategoryDto> CreateAsync(CreateSkillCategoryDto dto);
        public Task<IEnumerable<SkillCategoryDto>> GetSkillsList();
        public Task<SkillCategoryDto?> GetCategoryById(Guid id);
        Task<SkillCategoryDto?> ModifySkillsAsync(Guid categoryId, ModifySkillsDto dto);
    }
}
