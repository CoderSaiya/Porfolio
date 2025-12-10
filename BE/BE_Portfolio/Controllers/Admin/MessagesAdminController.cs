using BE_Portfolio.DTOs.Contact;
using BE_Portfolio.Models.Domain;
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
    public async Task<ActionResult<object>> GetMessages([FromQuery] MessageFilterQueryDto filter, CancellationToken ct)
    {
        var domainFilter = new MessageFilter
        {
            Status = filter.Status,
            SearchTerm = filter.SearchTerm,
            Page = filter.Page,
            PageSize = filter.PageSize
        };

        var messages = await repo.ListAsync(domainFilter, ct);
        var total = await repo.CountAsync(domainFilter, ct);

        var dto = messages.Select(m => new ContactMessageResponseDto(
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

        var dto = new ContactMessageResponseDto(
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
    public async Task<IActionResult> UpdateStatus(string id, [FromBody] UpdateMessageStatusRequestDto dto, CancellationToken ct)
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
    public async Task<IActionResult> BulkDelete([FromBody] BulkDeleteRequestDto dto, CancellationToken ct)
    {
        if (dto.Ids == null || !dto.Ids.Any()) return BadRequest("No IDs provided");
        await repo.BulkDeleteAsync(dto.Ids, ct);
        return NoContent();
    }
}
