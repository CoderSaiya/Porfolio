using BE_Portfolio.DTOs.Skill;
using BE_Portfolio.Models.Documents;
using BE_Portfolio.Persistence.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace BE_Portfolio.Controllers.Admin;

[ApiController]
[Route("api/admin/skills")]
[Authorize(Roles = "Admin")]
public class SkillsAdminController : ControllerBase
{
    private readonly ISkillRepository _skillRepo;

    public SkillsAdminController(ISkillRepository skillRepo)
    {
        _skillRepo = skillRepo;
    }

    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory([FromBody] CreateSkillCategoryRequestDto dto, CancellationToken ct)
    {
        try
        {
            var category = new SkillCategory
            {
                Title = dto.Title,
                Icon = dto.Icon,
                Color = dto.Color,
                Order = dto.Order,
                Skills = dto.Skills.Select(s => new SkillItem
                {
                    Name = s.Name,
                    Level = s.Level,
                    Order = s.Order
                }).ToList()
            };

            await _skillRepo.CreateCategoryAsync(category, ct);

            return CreatedAtAction(nameof(CreateCategory), new { id = category.Id }, category);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error creating skill category", error = ex.Message });
        }
    }

    [HttpPut("categories/{id}")]
    public async Task<IActionResult> UpdateCategory(string id, [FromBody] UpdateSkillCategoryRequestDto dto, CancellationToken ct)
    {
        try
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return BadRequest(new { message = "Invalid category ID" });

            var category = await _skillRepo.GetCategoryByIdAsync(objectId, ct);
            if (category == null)
                return NotFound(new { message = "Category not found" });

            // Update fields
            if (dto.Title != null) category.Title = dto.Title;
            if (dto.Icon != null) category.Icon = dto.Icon;
            if (dto.Color != null) category.Color = dto.Color;
            if (dto.Order.HasValue) category.Order = dto.Order.Value;
            
            if (dto.Skills != null)
            {
                category.Skills = dto.Skills.Select(s => new SkillItem
                {
                    Name = s.Name ?? "",
                    Level = s.Level ?? 0,
                    Order = s.Order ?? 0
                }).ToList();
            }

            await _skillRepo.UpdateCategoryAsync(category, ct);

            return Ok(category);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error updating skill category", error = ex.Message });
        }
    }

    [HttpDelete("categories/{id}")]
    public async Task<IActionResult> DeleteCategory(string id, CancellationToken ct)
    {
        try
        {
            if (!ObjectId.TryParse(id, out var objectId))
                return BadRequest(new { message = "Invalid category ID" });

            await _skillRepo.DeleteCategoryAsync(objectId, ct);

            return Ok(new { message = "Category deleted successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error deleting category", error = ex.Message });
        }
    }

    [HttpPost("categories/reorder")]
    public async Task<IActionResult> ReorderCategories([FromBody] ReorderCategoriesDTO dto, CancellationToken ct)
    {
        try
        {
            foreach (var item in dto.Categories)
            {
                if (ObjectId.TryParse(item.Id, out var objectId))
                {
                    await _skillRepo.UpdateCategoryOrderAsync(objectId, item.Order, ct);
                }
            }

            return Ok(new { message = "Categories reordered successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error reordering categories", error = ex.Message });
        }
    }
}
