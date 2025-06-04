using System.ComponentModel.DataAnnotations;

namespace OrderProcessingService.Models;

public class OrderItem
{
    [Key]
    public int Id { get; set; }

    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }

    public Guid OrderId { get; set; }
    public Order Order { get; set; } = null!;
}
