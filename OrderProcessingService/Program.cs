
using Microsoft.EntityFrameworkCore;
using OrderProcessingService.Data;
using OrderProcessingService.Hubs;
using OrderProcessingService.Services;
using System.Threading.Tasks;
using System;
using OrderProcessingService.Models;
using System.Security.Claims;

namespace OrderProcessingService;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new() { Title = "OrderProcessingService", Version = "v1" });

            c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Description = "Wpisz token: **Bearer test123**"
            });

            c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
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

        //to rówież dlatego, że dockera nie ma
        app.Use(async (context, next) =>
        {
            if (context.Request.Headers.TryGetValue("Authorization", out var token)
                && token.ToString() == "Bearer test123")
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "1"), // UserId
            new Claim(ClaimTypes.Name, "demo"),
            new Claim(ClaimTypes.Email, "demo@example.com")
        };

                var identity = new ClaimsIdentity(claims, "Fake");
                context.User = new ClaimsPrincipal(identity);
            }

            await next();
        });

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

