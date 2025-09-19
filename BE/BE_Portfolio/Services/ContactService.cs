using BE_Portfolio.Models.Commons;
using BE_Portfolio.Models.Documents;
using BE_Portfolio.Models.Specification;
using BE_Portfolio.Models.ValueObjects;
using BE_Portfolio.Persistence.Repositories.Interfaces;
using BE_Portfolio.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace BE_Portfolio.Services;

public class ContactService(
    IContactMessageRepository repo,
    IEmailQueue emailQueue,
    IOptions<ContactSettings> contactOptions,
    ILogger<ContactService> logger)
{
    private readonly ContactSettings _contactCfg = contactOptions.Value;

    public async Task SubmitAsync(ContactMessage doc, CancellationToken ct = default)
    {
        doc.CreatedAt = DateTime.UtcNow;
        doc.Status = MessageStatus.New;
        await repo.InsertAsync(doc, ct);
        
        var adminEmail = BuildAdminNotifyEmail(doc, _contactCfg);
        await emailQueue.EnqueueAsync(adminEmail, ct);
        
        if (!string.IsNullOrWhiteSpace(doc.Email))
        {
            var ackEmail = BuildUserAckEmail(doc);
            await emailQueue.EnqueueAsync(ackEmail, ct);
        }

        logger.LogInformation("Contact submitted and emails enqueued. Id={Id}", doc.Id);
    }


    public Task<List<ContactMessage>> ListAsync(MessageStatus? status, int? limit, CancellationToken ct = default)
        => repo.ListAsync(status, limit, ct);

    public Task SetStatusAsync(string id, MessageStatus status, CancellationToken ct = default)
        => repo.UpdateStatusAsync(id, status, ct);
    
    private static EmailMessage BuildAdminNotifyEmail(ContactMessage c, ContactSettings cfg)
    {
        var subject = $"[Contact] {c.Subject ?? "Tin nhắn mới"} từ {c.Name ?? c.Email}";
        var body = $@"
    <!DOCTYPE html>
    <html>
    <head>
        <meta charset='utf-8'>
        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
        <style>
            body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 20px; background-color: #f5f5f5; }}
            .container {{ max-width: 600px; margin: 0 auto; background: white; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); overflow: hidden; }}
            .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px 20px; text-align: center; }}
            .header h2 {{ margin: 0; font-size: 24px; font-weight: 300; }}
            .content {{ padding: 30px; }}
            .info-row {{ display: flex; margin-bottom: 15px; border-bottom: 1px solid #eee; padding-bottom: 12px; }}
            .info-label {{ font-weight: 600; color: #555; width: 80px; flex-shrink: 0; }}
            .info-value {{ color: #333; flex: 1; }}
            .message-box {{ background: #f8f9ff; border-left: 4px solid #667eea; padding: 20px; margin: 20px 0; border-radius: 0 8px 8px 0; }}
            .footer {{ background: #f8f9fa; padding: 20px; text-align: center; font-size: 12px; color: #6c757d; border-top: 1px solid #dee2e6; }}
            .badge {{ display: inline-block; background: #28a745; color: white; padding: 4px 12px; border-radius: 15px; font-size: 11px; font-weight: 500; }}
        </style>
    </head>
    <body>
        <div class='container'>
            <div class='header'>
                <h2>📧 Tin nhắn liên hệ mới</h2>
            </div>
            <div class='content'>
                <div class='info-row'>
                    <div class='info-label'>👤 Tên:</div>
                    <div class='info-value'>{c.Name ?? "Không có"}</div>
                </div>
                <div class='info-row'>
                    <div class='info-label'>✉️ Email:</div>
                    <div class='info-value'><a href='mailto:{c.Email}' style='color: #667eea; text-decoration: none;'>{c.Email}</a></div>
                </div>
                <div class='info-row'>
                    <div class='info-label'>📋 Tiêu đề:</div>
                    <div class='info-value'>{c.Subject ?? "Không có tiêu đề"}</div>
                </div>
                <div class='message-box'>
                    <div style='font-weight: 600; margin-bottom: 10px; color: #555;'>💬 Nội dung tin nhắn:</div>
                    <div>{System.Net.WebUtility.HtmlEncode(c.Message).Replace("\n", "<br/>")}</div>
                </div>
            </div>
            <div class='footer'>
                <span class='badge'>ID: {c.Id}</span> | 
                📅 Tạo lúc: {c.CreatedAt:dd/MM/yyyy HH:mm:ss}
            </div>
        </div>
    </body>
    </html>";
        
        return new EmailMessage
        {
            ToName = cfg.AdminName,
            ToEmail = cfg.AdminEmail,
            Subject = subject,
            Body = body
        };
    }

    private static EmailMessage BuildUserAckEmail(ContactMessage c)
    {
        var subject = "✅ Cảm ơn bạn đã liên hệ - Chúng tôi đã nhận được tin nhắn";
        var body = $@"
    <!DOCTYPE html>
    <html>
    <head>
        <meta charset='utf-8'>
        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
        <style>
            body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; line-height: 1.6; color: #333; margin: 0; padding: 20px; background-color: #f5f5f5; }}
            .container {{ max-width: 600px; margin: 0 auto; background: white; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); overflow: hidden; }}
            .header {{ background: linear-gradient(135deg, #28a745 0%, #20c997 100%); color: white; padding: 30px 20px; text-align: center; }}
            .header h2 {{ margin: 0; font-size: 24px; font-weight: 300; }}
            .content {{ padding: 30px; }}
            .greeting {{ font-size: 16px; margin-bottom: 20px; }}
            .main-text {{ background: linear-gradient(135deg, #e3f2fd 0%, #f3e5f5 100%); padding: 20px; border-radius: 8px; margin: 20px 0; }}
            .message-review {{ background: #fff3cd; border: 1px solid #ffeeba; border-radius: 8px; padding: 20px; margin: 20px 0; }}
            .message-review h4 {{ color: #856404; margin: 0 0 15px 0; }}
            .signature {{ margin-top: 30px; padding-top: 20px; border-top: 2px solid #e9ecef; }}
            .signature-name {{ font-weight: 600; color: #495057; font-size: 16px; }}
            .footer {{ background: #f8f9fa; padding: 15px 20px; text-align: center; font-size: 12px; color: #6c757d; }}
            .checkmark {{ font-size: 48px; text-align: center; margin: 20px 0; }}
        </style>
    </head>
    <body>
        <div class='container'>
            <div class='header'>
                <div class='checkmark'>✅</div>
                <h2>Cảm ơn bạn đã liên hệ!</h2>
            </div>
            <div class='content'>
                <div class='greeting'>
                    Chào <strong>{c.Name ?? c.Email}</strong>,
                </div>
                
                <div class='main-text'>
                    <p>🎉 <strong>Tin nhắn của bạn đã được gửi thành công!</strong></p>
                    <p>Chúng tôi đã nhận được và sẽ phản hồi trong thời gian sớm nhất (thường trong vòng 24 giờ).</p>
                </div>
    
                <div class='message-review'>
                    <h4>📝 Nội dung bạn đã gửi:</h4>
                    <div style='background: white; padding: 15px; border-radius: 5px; border-left: 4px solid #ffc107;'>
                        {System.Net.WebUtility.HtmlEncode(c.Message).Replace("\n", "<br/>")}
                    </div>
                </div>
    
                <div class='signature'>
                    <p>Trân trọng,</p>
                    <div class='signature-name'>💼 Nhật Cường</div>
                    <p style='font-size: 12px; color: #6c757d; margin-top: 5px;'>
                        Nếu cần hỗ trợ gấp, vui lòng gọi trực tiếp hoặc gửi email khác.
                    </p>
                </div>
            </div>
            <div class='footer'>
                🕐 Email tự động được gửi lúc: {DateTime.Now:dd/MM/yyyy HH:mm:ss}
            </div>
        </div>
    </body>
    </html>";
    
        return new EmailMessage
        {
            ToName = c.Name ?? c.Email,
            ToEmail = c.Email,
            Subject = subject,
            Body = body
        };
    }
}