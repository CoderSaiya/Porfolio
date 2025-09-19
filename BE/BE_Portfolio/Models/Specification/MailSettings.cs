namespace BE_Portfolio.Models.Specification;

public class MailSettings
{
    public string Host { get; set; } = null!;
    public int Port { get; set; }
    public string User { get; set; } = null!;
    public string Pass { get; set; } = null!;
    public string SenderName { get; set; } = null!;
    public string SenderEmail { get; set; } = null!;
}