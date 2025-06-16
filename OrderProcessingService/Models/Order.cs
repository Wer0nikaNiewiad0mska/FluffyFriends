using System.ComponentModel.DataAnnotations;

namespace OrderProcessingService.Models;

public class Order
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();
    public int UserId { get; set; }
    public List<OrderItem> Items { get; set; } = new();
    public decimal Total { get; set; }
    public string Status { get; set; } = "Pending";
}
