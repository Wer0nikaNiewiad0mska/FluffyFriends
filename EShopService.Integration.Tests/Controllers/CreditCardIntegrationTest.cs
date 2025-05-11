using Microsoft.VisualStudio.TestPlatform.TestHost;
using EShop.Domain.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http.Json;

namespace EShopService.Integration.Tests.Controllers;

public class CreditCardControllerIntegrationTest : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public CreditCardControllerIntegrationTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services
                    .SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<DataContext>));
                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }
                services.AddDbContext<DataContext>(options =>
                    options.UseInMemoryDatabase("MyDBForTest"));
            });
        });

        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task Validate_ValidVisaCard_ReturnsValidResponse()
    {
        string cardNumber = "4111 1111 1111 1111";
        var response = await _client.GetAsync($"/api/creditcard/validate?cardNumber={Uri.EscapeDataString(cardNumber)}");

        response.EnsureSuccessStatusCode();
        var validationResult = await response.Content.ReadFromJsonAsync<CreditCardValidationResponse>();
        Assert.NotNull(validationResult);
        Assert.Equal("valid", validationResult?.status);
        Assert.Equal("Visa", validationResult?.issuer);
    }

    [Fact]
    public async Task Validate_CardTooShort_ReturnsBadRequest()
    {
        string cardNumber = "4111 1111 111";
        var response = await _client.GetAsync($"/api/creditcard/validate?cardNumber={Uri.EscapeDataString(cardNumber)}");

        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        var message = await response.Content.ReadAsStringAsync();
        Assert.Equal("Card number is too short.", message);
    }

    [Fact]
    public async Task Validate_CardTooLong_ReturnsStatus414()
    {
        string cardNumber = "4111 1111 1111 1111 1111 1";
        var response = await _client.GetAsync($"/api/creditcard/validate?cardNumber={Uri.EscapeDataString(cardNumber)}");

        Assert.Equal((System.Net.HttpStatusCode)414, response.StatusCode);
        var message = await response.Content.ReadAsStringAsync();
        Assert.Equal("Card number is too long.", message);
    }

    [Fact]
    public async Task Validate_InvalidCardNumber_ReturnsStatus406()
    {
        string cardNumber = "4111 1111 1111 1112";
        var response = await _client.GetAsync($"/api/creditcard/validate?cardNumber={Uri.EscapeDataString(cardNumber)}");

        Assert.Equal((System.Net.HttpStatusCode)406, response.StatusCode);
        var message = await response.Content.ReadAsStringAsync();
        Assert.Equal("Card number is invalid.", message);
    }

    public class CreditCardValidationResponse
    {
        public string status { get; set; }
        public string issuer { get; set; }
    }
}
