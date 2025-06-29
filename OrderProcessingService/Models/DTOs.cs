﻿namespace OrderProcessingService.Models;

public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class OrderItemDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string ProductName { get; set; } = "";
}

public class ProcessOrderRequest
{
    public List<OrderItemDto> Items { get; set; } = new();
    public string? DiscountCode { get; set; } 
}
public class UserDto
{
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
}
