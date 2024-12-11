using BE_Portfolio.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BE_Portfolio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectController : Controller
    {
        private readonly IProject _projectService;

        public ProjectController(IProject projectService)
        {
            _projectService = projectService;
        }

        [HttpGet]
        public async Task<IActionResult> GetProjectsWithTags()
        {
            var result = await _projectService.GetProjects();
            if (result == null)
            {
                return BadRequest("List empty!");
            }
            return Ok(result);
        }
    }
}
