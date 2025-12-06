using BE_Portfolio.DTOs.Admin;
using BE_Portfolio.Models.Documents;
using BE_Portfolio.Models.ValueObjects;
using BE_Portfolio.Persistence.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BE_Portfolio.Controllers.Admin;

[ApiController]
[Route("api/admin/messages")]
// [Authorize(Roles = "Admin")]
public class MessagesAdminController(IContactMessageRepository repo) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<object>> GetMessages([FromQuery] MessageFilterDTO filter, CancellationToken ct)
    {
        var messages = await repo.ListAsync(filter.Status, filter.SearchTerm, filter.Page, filter.PageSize, ct);
        var total = await repo.CountAsync(filter.Status, filter.SearchTerm, ct);

        var dto = messages.Select(m => new MessageResponseDTO(
            m.Id.ToString(),
            m.Name,
            m.Email,
            m.Subject,
            m.Message,
            m.Status,
            m.Ip,
            m.UserAgent,
            m.ReadAt,
            m.CreatedAt
        ));

        return Ok(new
        {
            Data = dto,
            Total = total,
            Page = filter.Page,
            PageSize = filter.PageSize
        });
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ContactMessage>> GetMessage(string id, CancellationToken ct)
    {
        var msg = await repo.GetByIdAsync(id, ct);
        if (msg == null) return NotFound();

        var dto = new MessageResponseDTO(
            msg.Id.ToString(),
            msg.Name,
            msg.Email,
            msg.Subject,
            msg.Message,
            msg.Status,
            msg.Ip,
            msg.UserAgent,
            msg.ReadAt,
            msg.CreatedAt);
        return Ok(dto);
    }

    [HttpPut("{id}/status")]
    public async Task<IActionResult> UpdateStatus(string id, [FromBody] UpdateMessageStatusDTO dto, CancellationToken ct)
    {
        var msg = await repo.GetByIdAsync(id, ct);
        if (msg == null) return NotFound();

        await repo.UpdateStatusAsync(id, dto.Status, ct);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMessage(string id, CancellationToken ct)
    {
        await repo.DeleteAsync(id, ct);
        return NoContent();
    }

    [HttpDelete("bulk")]
    public async Task<IActionResult> BulkDelete([FromBody] BulkDeleteDTO dto, CancellationToken ct)
    {
        if (dto.Ids == null || !dto.Ids.Any()) return BadRequest("No IDs provided");
        await repo.BulkDeleteAsync(dto.Ids, ct);
        return NoContent();
    }
}
