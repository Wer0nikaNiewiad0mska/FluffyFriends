using OrderProcessingService.Data;
using OrderProcessingService.Models;

namespace OrderProcessingService.Services;

public class OrderProcessor
{
    private readonly OrderDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;

    private static readonly Dictionary<string, decimal> AvailableDiscounts = new()
    {
        { "PP4", 0.10m },
    };

    public OrderProcessor(OrderDbContext context, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<Guid> ProcessOrderAsync(int userId, ProcessOrderRequest request)
    {
        var client = _httpClientFactory.CreateClient();
        var user = new UserDto
        {
            Email = "demo@example.com",
            Username = "demo"
        };

        var order = new Order
        {
            UserId = userId,
            Username = user?.Username ?? "Nieznany",
            Email = user?.Email ?? "brak@mail.com"
        };

        foreach (var item in request.Items)
        {
            var product = new ProductDto
            {
                Id = item.ProductId,
                Name = "Mock Produkt",
                Price = 9.99m
            };
            if (product == null)
                throw new Exception($"Produkt {item.ProductId} nie istnieje.");

            order.Items.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            });
        }

        order.Total = order.Items.Sum(i => i.Quantity * i.UnitPrice);

        if (!string.IsNullOrWhiteSpace(request.DiscountCode) &&
            AvailableDiscounts.TryGetValue(request.DiscountCode.ToUpper(), out var discount))
        {
            order.Total *= 1 - discount;
        }

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        return order.Id;
    }
}
