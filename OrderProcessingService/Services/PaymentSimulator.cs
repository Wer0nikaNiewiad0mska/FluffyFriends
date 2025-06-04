using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using OrderProcessingService.Data;
using OrderProcessingService.Hubs;

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
