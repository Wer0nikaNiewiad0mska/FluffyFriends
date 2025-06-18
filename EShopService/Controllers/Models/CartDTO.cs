namespace EShopService.Controllers.Models;

public class CartItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class CartDto
{
    public List<CartItemDto> Items { get; set; } = new();
}
