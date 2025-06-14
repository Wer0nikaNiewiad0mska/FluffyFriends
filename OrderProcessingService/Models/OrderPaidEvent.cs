namespace OrderProcessingService.Models;

public class OrderPaidEvent
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public DateTime PaidAt { get; set; }
    public string Email { get; set; } = "";
    public List<OrderItemDto> Items { get; set; } = new();
    public decimal Total { get; set; }
}