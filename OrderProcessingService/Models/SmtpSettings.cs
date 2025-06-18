namespace OrderProcessingService.Models;

public class SmtpSettings
{
    public string Host { get; set; } = "";
    public int Port { get; set; }
    public string User { get; set; } = "";
    public string Password { get; set; } = "";
    public bool EnableSsl { get; set; }
    public string From { get; set; } = "";
}
