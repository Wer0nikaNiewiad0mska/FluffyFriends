using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OrderProcessingService.Data;
using OrderProcessingService.Hubs;
using OrderProcessingService.Models;
using Confluent.Kafka;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace OrderProcessingService.Services;

public class PaymentSimulator : BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly IHubContext<OrderHub> _hubContext;

    public PaymentSimulator(IServiceProvider provider, IHubContext<OrderHub> hubContext)
    {
        _provider = provider;
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _provider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
            var generator = scope.ServiceProvider.GetRequiredService<ReceiptGenerator>();
            var emailService = scope.ServiceProvider.GetRequiredService<EmailService>();

            var pending = await db.Orders
                .Where(o => o.Status == "Pending")
                .Include(o => o.Items)
                .ToListAsync(stoppingToken);

            //Wyłączone zapytanie HTTP – zamiast tego mock żeby pokazać, że działa:
            var productMap = new Dictionary<int, string>
            {
                { 1, "Adaś the Shark" },
                { 2, "Kiki the Hamster" },
            };

            foreach (var order in pending)
            {
                await Task.Delay(3000, stoppingToken); // symulacja przetwarzania
                order.Status = "Paid";
                await db.SaveChangesAsync(stoppingToken);

                var evt = new OrderPaidEvent
                {
                    OrderId = order.Id,
                    UserId = order.UserId,
                    Email = order.Email,
                    Username = order.Username,
                    PaidAt = DateTime.UtcNow,
                    Total = order.Total,
                    Items = order.Items.Select(i => new OrderItemDto
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity,
                        ProductName = productMap.TryGetValue(i.ProductId, out var name)
                            ? name
                            : $"Produkt {i.ProductId}"
                    }).ToList()
                };

                var html = generator.Generate(evt);
                await emailService.SendAsync(evt.Email, "Twój paragon", html);

                await _hubContext.Clients.All.SendAsync("PaymentStatus", new
                {
                    orderId = order.Id,
                    status = order.Status
                }, cancellationToken: stoppingToken);
            }

            await Task.Delay(5000, stoppingToken);
        }
    }
}