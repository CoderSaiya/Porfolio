using BE_Portfolio.DTOs.Common;
using BE_Portfolio.DTOs.Project;
using BE_Portfolio.Models.ValueObjects;
using BE_Portfolio.Services;
using BE_Portfolio.Services.Common;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BE_Portfolio.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectController(IProjectService service, IImageService imageService) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy danh sách dự án", Description = "Có thể lọc theo highlight và giới hạn số lượng.")]
    [ProducesResponseType(typeof(IEnumerable<ProjectItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProjects([FromQuery] ProjectFilterQueryDto query, CancellationToken ct)
        => Ok(await service.GetProjectsAsync(query, ct));

    [HttpGet("{slug}")]
    [SwaggerOperation(Summary = "Lấy chi tiết dự án", Description = "Tìm dự án theo slug SEO-friendly.")]
    [ProducesResponseType(typeof(ProjectResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProject([FromRoute] string slug, CancellationToken ct)
    {
        var proj = await service.GetProjectBySlugAsync(slug, ct);
        return proj is null ? NotFound() : Ok(proj);
    }
    
    // Image Handling
    [HttpGet("{id}/image/{variant}")]
    [SwaggerOperation(Summary = "Lấy ảnh dự án", Description = "Trả về ảnh WebP.")]
    [Produces("image/webp", "image/jpeg", "image/png")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<IActionResult> GetProjectImage(string id, string variant, CancellationToken ct)
    {
        if (!Enum.TryParse<ImageVariant>(variant, true, out var v)) v = ImageVariant.Thumb;
        
        var res = await imageService.GetImageBytesAsync(ImageOwnerType.Project, id, v, ct);
        if (res == null) return NotFound();
        
        return File(res.Value.Bytes, res.Value.MimeType);
    }
    
    [HttpGet("{id}/image/{variant}/data-url")]
    public async Task<IActionResult> GetProjectImageDataUrl(string id, string variant, CancellationToken ct)
    {
        if (!Enum.TryParse<ImageVariant>(variant, true, out var v)) v = ImageVariant.Thumb;

        var dataUrl = await imageService.GetImageDataUrlAsync(ImageOwnerType.Project, id, v, ct);
        return dataUrl == null ? NotFound() : Ok(new ImageResponseDto(dataUrl));
    }
}
