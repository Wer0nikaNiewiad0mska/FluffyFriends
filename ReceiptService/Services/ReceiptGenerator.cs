using ReceiptService.Models;
using System.Text;

namespace ReceiptService.Services;

public class ReceiptGenerator
{
    public string Generate(OrderPaidEvent ev)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"<h2>Paragon {ev.OrderId}</h2>");
        sb.AppendLine($"<p>Data: {ev.PaidAt:yyyy-MM-dd}</p><ul>");
        foreach (var item in ev.Items)
            sb.AppendLine($"<li>Produkt {item.ProductId}, ilość: {item.Quantity}</li>");
        sb.AppendLine("</ul>");
        sb.AppendLine($"<strong>Łącznie: {ev.Total:C}</strong>");
        return sb.ToString();
    }
}
