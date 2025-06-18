using BE_Portfolio.DTOs;
using BE_Portfolio.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BE_Portfolio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillController(ISkillService skillService) : Controller
    {
        [HttpPost]
        public async Task<ActionResult<SkillCategoryDto>> Create(CreateSkillCategoryDto dto)
        {
            var created = await skillService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<SkillCategoryDto>> GetById(Guid id)
        {
            var cat = await skillService.GetCategoryById(id);
            if (cat is null) return NotFound();
            return Ok(cat);
        }
        
        [HttpGet]
        public async Task<IActionResult> GetSkillsList()
        {
            var result = await skillService.GetSkillsList();
            return Ok(result);
        }
        
        [HttpPatch("{id:int}/skills")]
        public async Task<ActionResult<SkillCategoryDto>> ModifySkills(
            Guid id, [FromBody] ModifySkillsDto dto)
        {
            var updated = await skillService.ModifySkillsAsync(id, dto);
            if (updated is null) return NotFound();
            return Ok(updated);
        }
    }
}
