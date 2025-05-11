using Microsoft.VisualStudio.TestPlatform.TestHost;
using System;
using System.Collections.Generic;
using EShop.Domain.Repositories;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net.Http.Json;
using EShop.Domain.Models;

namespace EShopService.Integration.Tests.Controllers;

public class ProductControllerIntegrationTest : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;

    public ProductControllerIntegrationTest(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var dbContextDescriptor = services
                    .SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<DataContext>));

                if (dbContextDescriptor != null)
                    services.Remove(dbContextDescriptor);

                services.AddDbContext<DataContext>(options =>
                    options.UseInMemoryDatabase("MyDBForTest"));
            });
        });

        _client = _factory.CreateClient();
    }

    // Reset bazy danych przed każdym testem
    public async Task InitializeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Get_ReturnsAllProducts()
    {
        // Arrange – seed danych testowych
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();

            var category = new Category
            {
                Name = "TestCategory",
                created_by = Guid.NewGuid(),
                updated_by = Guid.NewGuid()
            };

            await dbContext.Categories.AddAsync(category);
            await dbContext.SaveChangesAsync();

            var products = new List<Product>
            {
                new Product
                {
                    Name = "Product1",
                    ean = "1111111111111",
                    sku = "SKU-001",
                    price = 10.0m,
                    stock = 100,
                    category = category,
                    created_by = Guid.NewGuid(),
                    updated_by = Guid.NewGuid()
                },
                new Product
                {
                    Name = "Product2",
                    ean = "2222222222222",
                    sku = "SKU-002",
                    price = 20.0m,
                    stock = 50,
                    category = category,
                    created_by = Guid.NewGuid(),
                    updated_by = Guid.NewGuid()
                }
            };

            await dbContext.Products.AddRangeAsync(products);
            await dbContext.SaveChangesAsync();
        }

        // Act
        var response = await _client.GetAsync("/api/product");

        // Assert
        response.EnsureSuccessStatusCode();
        var productsFromApi = await response.Content.ReadFromJsonAsync<List<Product>>();

        Assert.NotNull(productsFromApi);
        Assert.Equal(2, productsFromApi!.Count);
        Assert.Contains(productsFromApi, p => p.Name == "Product1");
        Assert.Contains(productsFromApi, p => p.sku == "SKU-002");
    }
}
