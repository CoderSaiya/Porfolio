using BE_Portfolio.DTOs;
using BE_Portfolio.Models.Documents;
using BE_Portfolio.Models.ValueObjects;
using BE_Portfolio.Services;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace BE_Portfolio.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContactController(ContactService svc) : Controller
{
    [HttpPost]
    [SwaggerOperation(Summary = "Gửi liên hệ", Description = "Lưu message vào Mongo (trạng thái NEW).")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Submit([FromBody] ContactCreateReq req, CancellationToken ct)
    {
        if (!ModelState.IsValid) return ValidationProblem(ModelState);

        var doc = new ContactMessage
        {
            Name = req.Name.Trim(),
            Email = req.Email.Trim(),
            Subject = req.Subject.Trim(),
            Message = req.Message.Trim(),
            Status = MessageStatus.New,
            Ip = HttpContext.Connection.RemoteIpAddress?.ToString(),
            UserAgent = Request.Headers.UserAgent.ToString(),
        };

        await svc.SubmitAsync(doc, ct);
        return Created($"/api/contact/{doc.Id}", new { ok = true, id = doc.Id.ToString() });
    }
    
    [HttpGet]
    [SwaggerOperation(Summary = "Danh sách liên hệ", Description = "Dùng cho admin dashboard, có thể lọc status & limit.")]
    [ProducesResponseType(typeof(List<ContactMessage>), StatusCodes.Status200OK)]
    public async Task<IActionResult> List([FromQuery] MessageStatus? status, [FromQuery] int? limit, CancellationToken ct)
        => Ok(await svc.ListAsync(status, limit, ct));
    
    [HttpPatch("{id}/status/{status:alpha}")]
    [SwaggerOperation(Summary = "Cập nhật trạng thái", Description = "Đặt trạng thái message: New/Read/Archived.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> SetStatus([FromRoute] string id, [FromRoute] string status, CancellationToken ct)
    {
        if (!Enum.TryParse<MessageStatus>(status, true, out var s))
            return ValidationProblem("Trạng thái không hợp lệ.");
        await svc.SetStatusAsync(id, s, ct);
        return NoContent();
    }
}