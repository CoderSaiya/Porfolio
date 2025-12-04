using BE_Portfolio.DTOs.Admin;
using BE_Portfolio.Models.Documents;
using BE_Portfolio.Persistence.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace BE_Portfolio.Controllers.Admin;

[ApiController]
[Route("api/admin/projects")]
[Authorize(Roles = "Admin")]
public class ProjectAdminController : ControllerBase
{
    private readonly IProjectRepository _projectRepo;
    private readonly IImageRepository _imageRepo;

    public ProjectAdminController(IProjectRepository projectRepo, IImageRepository imageRepo)
    {
        _projectRepo = projectRepo;
        _imageRepo = imageRepo;
    }

    [HttpPost]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectDTO dto, CancellationToken ct)
    {
        try
        {
            var project = new Project
            {
                Title = dto.Title,
                Slug = dto.Slug,
                Description = dto.Description,
                Highlight = dto.Highlight,
                Duration = dto.Duration,
                TeamSize = dto.TeamSize,
                Github = dto.Github,
                Demo = dto.Demo,
                Technologies = dto.Technologies,
                Features = dto.Features,
                CreatedAt = DateTime.UtcNow,
                ImageUrl = $"/api/portfolio/projects/{ObjectId.GenerateNewId()}/image/thumb" // Placeholder
            };

            await _projectRepo.CreateAsync(project, ct);

            return CreatedAtAction(nameof(CreateProject), new { id = project.Id }, project);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error creating project", error = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(string id, [FromBody] UpdateProjectDTO dto, CancellationToken ct)
    {
        try
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return BadRequest(new { message = "Invalid project ID" });

            var project = await _projectRepo.GetByIdAsync(objectId, ct);
            if (project == null)
                return NotFound(new { message = "Project not found" });

            // Update only provided fields
            if (dto.Title != null) project.Title = dto.Title;
            if (dto.Slug != null) project.Slug = dto.Slug;
            if (dto.Description != null) project.Description = dto.Description;
            if (dto.Highlight.HasValue) project.Highlight = dto.Highlight.Value;
            if (dto.Duration.HasValue) project.Duration = dto.Duration.Value;
            if (dto.TeamSize.HasValue) project.TeamSize = dto.TeamSize.Value;
            if (dto.Github != null) project.Github = dto.Github;
            if (dto.Demo != null) project.Demo = dto.Demo;
            if (dto.Technologies != null) project.Technologies = dto.Technologies;
            if (dto.Features != null) project.Features = dto.Features;

            await _projectRepo.UpdateAsync(project, ct);

            return Ok(project);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error updating project", error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(string id, CancellationToken ct)
    {
        try
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return BadRequest(new { message = "Invalid project ID" });

            var project = await _projectRepo.GetByIdAsync(objectId, ct);
            if (project == null)
                return NotFound(new { message = "Project not found" });

            // Delete associated images
            await _imageRepo.DeleteByOwnerAsync("Project", id, ct);

            // Delete project
            await _projectRepo.DeleteAsync(objectId, ct);

            return Ok(new { message = "Project deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error deleting project", error = ex.Message });
        }
    }

    [HttpPost("{id}/image")]
    public async Task<IActionResult> UploadProjectImage(string id, [FromForm] IFormFile file, [FromQuery] string variant = "thumb", CancellationToken ct = default)
    {
        try
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return BadRequest(new { message = "Invalid project ID" });

            var project = await _projectRepo.GetByIdAsync(objectId, ct);
            if (project == null)
                return NotFound(new { message = "Project not found" });

            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file provided" });

            // Validate file type
            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowedTypes.Contains(file.ContentType))
                return BadRequest(new { message = "Invalid file type. Only JPEG, PNG, and WebP are allowed" });

            // Read file to byte array
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, ct);
            var imageData = memoryStream.ToArray();

            // Create or update image
            var image = new Image
            {
                OwnerType = "Project",
                OwnerId = id,
                Variant = variant,
                Data = imageData,
                ContentType = file.ContentType,
                UploadedAt = DateTime.UtcNow
            };

            await _imageRepo.UpsertAsync(image, ct);

            return Ok(new { message = "Image uploaded successfully", imageUrl = $"/api/portfolio/projects/{id}/image/{variant}" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error uploading image", error = ex.Message });
        }
    }
}
