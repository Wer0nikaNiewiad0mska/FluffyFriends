
using Microsoft.EntityFrameworkCore;
using OrderProcessingService.Data;
using OrderProcessingService.Hubs;
using OrderProcessingService.Services;
using System.Threading.Tasks;
using System;
using OrderProcessingService.Models;

namespace OrderProcessingService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSignalR();
        builder.Services.AddDbContext<OrderDbContext>(opt =>
            opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        builder.Services.AddHttpClient();

        builder.Services.AddScoped<OrderProcessor>();
        builder.Services.AddHostedService<PaymentSimulator>();
        builder.Services.AddScoped<EmailService>();
        builder.Services.AddScoped<ReceiptGenerator>();
        builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));


        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
        app.MapHub<OrderHub>("/orderHub");

        app.Run();
    }
}
//Poradnik co do migracji
//dotnet ef migrations add [nazwa np.AddInvoices]
//dotnet ef database update

//jak często?
//Dodałeś nową klasę modelu(np.Invoice)
//Dodałeś nową właściwość do Order(np.DeliveryDate)
//Zmieniłeś typ istniejącej właściwości(np. string → decimal)   
//Zmieniłeś relacje między tabelami

