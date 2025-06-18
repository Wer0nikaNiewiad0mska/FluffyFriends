using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Options;
using OrderProcessingService.Models;

namespace OrderProcessingService.Services;

public class EmailService
{
    private readonly SmtpSettings _settings;

    public EmailService(IOptions<SmtpSettings> settings)
    {
        _settings = settings.Value;
    }

    public async Task SendAsync(string to, string subject, string body)
    {
        var message = new MailMessage(_settings.From, to, subject, body)
        {
            IsBodyHtml = true
        };

        using var client = new SmtpClient(_settings.Host)
        {
            Port = _settings.Port,
            Credentials = new NetworkCredential(_settings.User, _settings.Password),
            EnableSsl = _settings.EnableSsl
        };

        await client.SendMailAsync(message);
    }
}
