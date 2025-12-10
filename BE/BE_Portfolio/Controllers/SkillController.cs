using BE_Portfolio.Models.Documents;
using BE_Portfolio.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BE_Portfolio.Controllers;

[ApiController]
[Route("api/skills")]
public class SkillController(ISkillService service) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy kỹ năng", Description = "Danh sách skill.")]
    [ProducesResponseType(typeof(List<SkillCategory>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSkills(CancellationToken ct)
        => Ok(await service.GetSkillsAsync(ct));
}
