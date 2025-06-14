using System.Net.Mail;
using System.Net;

namespace ReceiptService.Services;

public class EmailService
{
    public async Task SendAsync(string to, string subject, string body)
    {
        var message = new MailMessage("noreply@eshop.com", to, subject, body)
        {
            IsBodyHtml = true
        };
        using var client = new SmtpClient("smtp.example.com")
        {
            Port = 587,
            Credentials = new NetworkCredential("user", "pass"),
            EnableSsl = true
        };
        await client.SendMailAsync(message);
    }
}
