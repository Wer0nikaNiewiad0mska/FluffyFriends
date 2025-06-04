using Microsoft.EntityFrameworkCore;
using OrderProcessingService.Models;
using System.Collections.Generic;

namespace OrderProcessingService.Data;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> Items => Set<OrderItem>();
}
