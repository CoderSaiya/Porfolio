using BE_Portfolio.Data;
using BE_Portfolio.Models;
using BE_Portfolio.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BE_Portfolio.Services
{
    public class SkillService : ISkill
    {
        private readonly PortfolioDbContext _context;

        public SkillService(PortfolioDbContext context)
        {
            _context = context;
        }

        public async Task<List<Skill>?> GetSkill()
        {
            var skillList = await _context.Skills.ToListAsync();
            if (!skillList.Any())
            {
                return null;
            }
            return skillList;
        }
    }
}
