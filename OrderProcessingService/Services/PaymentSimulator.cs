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
            var pending = await db.Orders.Where(o => o.Status == "Pending").ToListAsync(stoppingToken);

            foreach (var order in pending)
            {
                await Task.Delay(3000, stoppingToken); // symulacja przetwarzania
                order.Status = "Paid";
                await db.SaveChangesAsync(stoppingToken);

                var client = scope.ServiceProvider.GetRequiredService<HttpClient>();
                var user = await client.GetFromJsonAsync<UserDto>($"https://localhost:7001/api/users/{order.UserId}");

                var kafkaConfig = new ProducerConfig { BootstrapServers = "localhost:9092" };
                using var producer = new ProducerBuilder<Null, string>(kafkaConfig).Build();

                var evt = new OrderPaidEvent
                {
                    OrderId = order.Id,
                    UserId = order.UserId,
                    PaidAt = DateTime.UtcNow,
                    Email = user?.Email ?? "",
                    Total = order.Total,
                    Items = order.Items.Select(i => new OrderItemDto
                    {
                        ProductId = i.ProductId,
                        Quantity = i.Quantity
                    }).ToList()
                };

                await producer.ProduceAsync("order-paid", new Message<Null, string>
                {
                    Value = JsonConvert.SerializeObject(evt)
                });

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
