using OrderProcessingService.Models;
using System.Text;

namespace OrderProcessingService.Services;

public class ReceiptGenerator
{
    public string Generate(OrderPaidEvent ev)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"<h2>Paragon {ev.OrderId}</h2>");
        sb.AppendLine($"<p>Data: {ev.PaidAt:yyyy-MM-dd}</p>");
        sb.AppendLine($"<p>Klient: {ev.Username} ({ev.Email})</p>");
        sb.AppendLine("<ul>");
        foreach (var item in ev.Items)
            sb.AppendLine($"<li>{item.ProductName} – ilość: {item.Quantity}</li>");
        sb.AppendLine("</ul>");
        sb.AppendLine($"<strong>Łącznie: {ev.Total:C}</strong>");
        return sb.ToString();
    }
}
