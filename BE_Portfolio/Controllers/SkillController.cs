using BE_Portfolio.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BE_Portfolio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillController : Controller
    {
        private readonly ISkill _skillService;

        public SkillController(ISkill skillService)
        {
            _skillService = skillService;
        }

        [HttpGet]
        public async Task<IActionResult> GetSkillsList()
        {
            var result = await _skillService.GetSkill();
            if (result == null)
            {
                return BadRequest("List empty!");
            }
            return Ok(result);
        }
    }
}
