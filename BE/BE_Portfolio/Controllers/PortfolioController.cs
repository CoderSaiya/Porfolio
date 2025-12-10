using BE_Portfolio.DTOs.Common;
using BE_Portfolio.DTOs.Project;
using BE_Portfolio.Models.Documents;
using BE_Portfolio.Models.ValueObjects;
using BE_Portfolio.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Swashbuckle.AspNetCore.Annotations;

namespace BE_Portfolio.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PortfolioController(PortfolioService svc) : Controller
{
    [HttpGet("profile")]
    [SwaggerOperation(Summary = "Lấy profile", Description = "Trả về hồ sơ cá nhân hiển thị ở trang About/Hero.")]
    [ProducesResponseType(typeof(Profile), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
        => Ok(await svc.GetProfileAsync(ct));
    
    [HttpGet("skills")]
    [SwaggerOperation(Summary = "Lấy kỹ năng", Description = "Danh sách các nhóm kỹ năng và từng skill với mức độ.")]
    [ProducesResponseType(typeof(List<SkillCategory>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSkills(CancellationToken ct)
        => Ok(await svc.GetSkillsAsync(ct));
    
    [HttpGet("projects")]
    [SwaggerOperation(Summary = "Lấy danh sách dự án", Description = "Có thể lọc theo highlight và giới hạn số lượng.")]
    [ProducesResponseType(typeof(IEnumerable<ProjectItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProjects([FromQuery] bool? highlight, [FromQuery] int? limit, CancellationToken ct)
        => Ok(await svc.GetProjectsAsync(highlight, limit, ct));
    
    [HttpGet("projects/{slug}")]
    [SwaggerOperation(Summary = "Lấy chi tiết dự án", Description = "Tìm dự án theo slug SEO-friendly.")]
    [ProducesResponseType(typeof(ProjectResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetProject([FromRoute] string slug, CancellationToken ct)
    {
        var proj = await svc.GetProjectBySlugAsync(slug, ct);
        return proj is null ? NotFound() : Ok(proj);
    }
    
    [HttpPost("projects/{id}/images")]
    [Consumes("multipart/form-data")]
    [SwaggerOperation(
        Summary = "Upload ảnh dự án",
        Description = "Nhận IFormFile, convert về WebP, lưu Full (<=1920px) và Thumb (<=480px).",
        OperationId = "UploadProjectImage")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadProjectImage(
        [FromRoute] string id,
        [FromForm] UploadProjectImageRequestDto req,
        CancellationToken ct)
    {
        if (!ObjectId.TryParse(id, out var objId))
            return ValidationProblem("Id không hợp lệ.");

        await svc.SetProjectImageAsync(objId, req.File, ct);
        return NoContent();
    }
    
    [HttpGet("projects/{id}/image/{variant:alpha}")]
    [SwaggerOperation(Summary = "Lấy ảnh bytes", Description = "Trả file bytes (image/webp). Hợp để tải nhanh qua CDN/browser cache.")]
    [Produces("image/webp", "image/jpeg", "image/png")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<IActionResult> GetProjectImageFile(
        [FromRoute] string id,
        [FromRoute] string variant,
        CancellationToken ct)
    {
        if (!ObjectId.TryParse(id, out var objId))
            return BadRequest();

        if (!Enum.TryParse<ImageVariant>(variant, true, out var vv))
            vv = ImageVariant.Thumb;

        var res = await svc.GetProjectImageBytesAsync(objId, vv, ct);
        if (res is null) return NotFound();

        var (bytes, mime) = res.Value;
        return File(bytes, mime);
    }

    [HttpGet("projects/{id}/image/{variant:alpha}/data-url")]
    [SwaggerOperation(Summary = "Lấy ảnh data-url", Description = "Trả về chuỗi Data URL (data:&lt;mime&gt;;base64,...)")]
    [ProducesResponseType(typeof(ImageResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<IActionResult> GetProjectImageDataUrl(
        [FromRoute] string id,
        [FromRoute] string variant,
        CancellationToken ct)
    {
        if (!ObjectId.TryParse(id, out var objId))
            return BadRequest();

        if (!Enum.TryParse<ImageVariant>(variant, true, out var vv))
            vv = ImageVariant.Thumb;

        var dataUrl = await svc.GetProjectImageDataUrlAsync(objId, vv, ct);
        return dataUrl is null
            ? NotFound()
            : Ok(new ImageResponseDto(DataUrl: dataUrl));
    }
    
    [HttpDelete("projects/{id}/image/{variant:alpha}")]
    [SwaggerOperation(Summary = "Xoá ảnh dự án", Description = "Xoá ảnh theo Owner=Project + Variant.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeleteProjectImage(
        [FromRoute] string id,
        [FromRoute] string variant,
        CancellationToken ct)
    {
        if (!ObjectId.TryParse(id, out var objId))
            return BadRequest();

        if (!Enum.TryParse<ImageVariant>(variant, true, out var vv))
            vv = ImageVariant.Thumb;

        await svc.DeleteProjectImageAsync(objId, vv, ct);
        return NoContent();
    }
    [HttpGet("profile/image/{variant:alpha}")]
    [SwaggerOperation(Summary = "Lấy ảnh profile", Description = "Trả file bytes (image/webp) của profile avatar.")]
    [Produces("image/webp", "image/jpeg", "image/png")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<IActionResult> GetProfileImageFile(
        [FromRoute] string variant,
        CancellationToken ct)
    {
        if (!Enum.TryParse<ImageVariant>(variant, true, out var vv))
            vv = ImageVariant.Thumb;

        var res = await svc.GetProfileImageBytesAsync(vv, ct);
        if (res is null) return NotFound();

        var (bytes, mime) = res.Value;
        return File(bytes, mime);
    }
}