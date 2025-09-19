using BE_Portfolio.Models.Commons;

namespace BE_Portfolio.Services.Interfaces;

public interface IMailSender
{
    Task SendEmailAsync(EmailMessage message);
}