using BE_Portfolio.Models.Commons;
using BE_Portfolio.Models.Specification;
using BE_Portfolio.Services.Interfaces;
using Microsoft.Extensions.Options;
using MimeKit;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;

namespace BE_Portfolio.Services;

public class EmailSender(IOptions<MailSettings> opts) : IMailSender
{
    private readonly MailSettings _settings = opts.Value;

    public async Task SendEmailAsync(EmailMessage message)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
        email.To.Add(new MailboxAddress(message.ToName, message.ToEmail));
        email.Subject = message.Subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = message.Body };
        email.Body = bodyBuilder.ToMessageBody();

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.Host, _settings.Port, MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_settings.User, _settings.Pass);
        await client.SendAsync(email);
        await client.DisconnectAsync(true);
    }
}