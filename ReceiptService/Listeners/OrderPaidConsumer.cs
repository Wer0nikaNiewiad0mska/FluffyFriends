using Confluent.Kafka;
using Newtonsoft.Json;
using ReceiptService.Models;
using ReceiptService.Services;

namespace ReceiptService.Listeners;

public class OrderPaidConsumer : BackgroundService
{
    private readonly IServiceProvider _provider;

    public OrderPaidConsumer(IServiceProvider provider)
    {
        _provider = provider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            GroupId = "receipt-service",
            BootstrapServers = "localhost:9092",
            AutoOffsetReset = AutoOffsetReset.Earliest
        };

        using var consumer = new ConsumerBuilder<Ignore, string>(config).Build();
        consumer.Subscribe("order-paid");

        while (!stoppingToken.IsCancellationRequested)
        {
            var result = consumer.Consume(stoppingToken);
            var data = JsonConvert.DeserializeObject<OrderPaidEvent>(result.Message.Value)!;

            using var scope = _provider.CreateScope();
            var generator = scope.ServiceProvider.GetRequiredService<ReceiptGenerator>();
            var emailer = scope.ServiceProvider.GetRequiredService<EmailService>();

            var html = generator.Generate(data);
            await emailer.SendAsync(data.Email, "Twój paragon", html);
        }
    }
}
