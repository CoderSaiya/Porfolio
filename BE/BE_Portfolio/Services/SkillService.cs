using BE_Portfolio.Data;
using BE_Portfolio.DTOs;
using BE_Portfolio.Models.Entities;
using BE_Portfolio.Repositories.Interfaces;
using BE_Portfolio.Services.Interfaces;

namespace BE_Portfolio.Services
{
    public class SkillService(
        ISkillRepository skillRepository, 
        IUnitOfWork unitOfWork
        ) : ISkillService
    {
        public async Task<SkillCategoryDto> CreateAsync(CreateSkillCategoryDto dto)
        {
            var entity = new SkillCategory
            {
                Title = dto.Title,
                IconName = dto.IconName,
                Color = dto.Color,
                Skills = dto.Skills.Select(s => new Skill { Name = s }).ToList()
            };
            var saved = await skillRepository.AddCategoryAsync(entity);
            await unitOfWork.CommitAsync();
            return MapToDto(saved);
        }

        public async Task<IEnumerable<SkillCategoryDto>> GetSkillsList()
        {
            var skillList = await skillRepository.GetSkillCategories();
            
            return MapToDtos(skillList);
        }

        public async Task<SkillCategoryDto?> GetCategoryById(Guid id)
        {
            var cat = await skillRepository.GetCategoryByIdAsync(id);
            return cat is null ? null : MapToDto(cat);
        }

        public async Task<SkillCategoryDto?> ModifySkillsAsync(Guid categoryId, ModifySkillsDto dto)
        {
            var cat = await skillRepository.GetCategoryByIdAsync(categoryId);
            if (cat is null) return null;
            
            // Thêm skills
            if (dto.AddSkills is { Count: > 0 })
            {
                foreach (var name in dto.AddSkills)
                {
                    if (!cat.Skills.Any(s => s.Name == name))
                        cat.Skills.Add(new Skill { Name = name });
                }
            }
            
            // Xóa skills
            if (dto.RemoveSkills is { Count: > 0 })
            {
                var toRemove = cat.Skills
                    .Where(s => dto.RemoveSkills.Contains(s.Name))
                    .ToList();
                foreach (var s in toRemove)
                    cat.Skills.Remove(s);
            }

            await unitOfWork.CommitAsync();
            return MapToDto(cat);
        }
        
        private static SkillCategoryDto MapToDto(SkillCategory c)
            => new(
                c.Id,
                c.Title,
                c.IconName,
                c.Color,
                c.Skills.Select(s => s.Name).ToList()
            );
        
        private static IEnumerable<SkillCategoryDto> MapToDtos(IEnumerable<SkillCategory> cs)
            => cs.Select(c => new SkillCategoryDto(
                c.Id,
                c.Title,
                c.IconName,
                c.Color,
                c.Skills.Select(s => s.Name).ToList()
            ));
    }
}
