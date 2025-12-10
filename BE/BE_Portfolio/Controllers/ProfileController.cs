using BE_Portfolio.Models.Documents;
using BE_Portfolio.Models.ValueObjects;
using BE_Portfolio.Services;
using BE_Portfolio.Services.Common;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BE_Portfolio.Controllers;

[ApiController]
[Route("api/profile")]
public class ProfileController(IProfileService service, IImageService imageService) : ControllerBase
{
    [HttpGet]
    [SwaggerOperation(Summary = "Lấy profile", Description = "Trả về hồ sơ cá nhân.")]
    [ProducesResponseType(typeof(Profile), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProfile(CancellationToken ct)
        => Ok(await service.GetProfileAsync(ct));

    [HttpGet("image/{variant}")]
    [SwaggerOperation(Summary = "Lấy ảnh profile", Description = "Trả về ảnh avatar.")]
    [ResponseCache(Duration = 3600, Location = ResponseCacheLocation.Any, NoStore = false)]
    public async Task<IActionResult> GetProfileImage(string variant, CancellationToken ct)
    {
        if (!Enum.TryParse<ImageVariant>(variant, true, out var v)) v = ImageVariant.Thumb;
        
        var profile = await service.GetProfileAsync(ct);
        if (profile == null) return NotFound();

        var imgRes = await imageService.GetImageBytesAsync(ImageOwnerType.Profile, profile.Id.ToString(), v, ct);
        if (imgRes == null) return NotFound();

        return File(imgRes.Value.Bytes, imgRes.Value.MimeType);
    }
}
