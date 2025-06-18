using BE_Portfolio.DTOs;
using BE_Portfolio.Models.Documents;
using BE_Portfolio.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BE_Portfolio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController(IProjectService projectService) : Controller
    {   
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> Get() =>
            Ok(await projectService.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> Get(string id) =>
            Ok(await projectService.GetByIdAsync(id));

        [HttpPost]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(10_000_000)]
        public async Task<ActionResult<Project>> Create([FromQuery] CreateProjectDto dto)
        {
            var project = await projectService.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id = project.Id }, project);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await projectService.DeleteAsync(id);
            return NoContent();
        }
    }
}
