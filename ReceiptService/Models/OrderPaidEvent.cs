namespace ReceiptService.Models;

public class OrderPaidEvent
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime PaidAt { get; set; }
    public decimal Total { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}

public class OrderItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}
