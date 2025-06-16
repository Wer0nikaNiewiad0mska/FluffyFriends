using EShop.Domain.Models;
using EShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EShop.Application.Tests;

public class ProductServiceTests
{
    private DataContext GetInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var context = new DataContext(options);
        context.Database.EnsureCreated();
        return context;
    }

    [Fact]
    public async Task AddProduct_ShouldAddProduct()
    {
        // Arrange
        var context = GetInMemoryContext();
        var repo = new ProductRepository(context);
        var service = new ProductService(repo);

        var category = new Category { Name = "TestCategory" };
        var product = new Product
        {
            Name = "Test Product",
            ean = "123456789",
            price = 99.99m,
            stock = 10,
            sku = "TEST-001",
            category = category,
            created_by = 1,
            updated_by = 1
        };

        // Act
        await service.AddAsync(product);
        var all = await service.GetAllAsync();

        // Assert
        Assert.Single(all);
        Assert.Equal("Test Product", all.First().Name);
    }

    [Fact]
    public async Task GetById_ShouldReturnCorrectProduct()
    {
        var context = GetInMemoryContext();
        var repo = new ProductRepository(context);
        var service = new ProductService(repo);

        var product = new Product
        {
            Name = "Test",
            ean = "123",
            price = 10,
            stock = 1,
            sku = "SKU-123",
            category = new Category { Name = "Books" },
            created_by = 1,
            updated_by = 1
        };

        await service.AddAsync(product);
        var result = await service.GetByIdAsync(product.id);

        Assert.NotNull(result);
        Assert.Equal("Test", result!.Name);
    }

    [Fact]
    public async Task UpdateProduct_ShouldChangeProductValues()
    {
        var context = GetInMemoryContext();
        var repo = new ProductRepository(context);
        var service = new ProductService(repo);

        var product = new Product
        {
            Name = "Original",
            ean = "321",
            price = 10,
            stock = 5,
            sku = "SKU-321",
            category = new Category { Name = "Electronics" },
            created_by = 1,
            updated_by = 1
        };

        await service.AddAsync(product);

        product.Name = "Updated";
        await service.UpdateAsync(product);

        var updated = await service.GetByIdAsync(product.id);
        Assert.Equal("Updated", updated!.Name);
    }

    [Fact]
    public async Task DeleteProduct_ShouldRemoveProduct()
    {
        var context = GetInMemoryContext();
        var repo = new ProductRepository(context);
        var service = new ProductService(repo);

        var product = new Product
        {
            Name = "ToDelete",
            ean = "000",
            price = 5,
            stock = 3,
            sku = "SKU-DEL",
            category = new Category { Name = "Fashion" },
            created_by = 1,
            updated_by = 1
        };

        await service.AddAsync(product);
        await service.DeleteAsync(product.id);

        var result = await service.GetByIdAsync(product.id);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetByCategory_ShouldReturnFilteredProducts()
    {
        var context = GetInMemoryContext();
        var repo = new ProductRepository(context);
        var service = new ProductService(repo);

        var category = new Category { Name = "Books" };

        await service.AddAsync(new Product
        {
            Name = "Book 1",
            ean = "111",
            price = 15,
            stock = 20,
            sku = "BOOK-001",
            category = category,
            created_by = 1,
            updated_by = 1
        });

        await service.AddAsync(new Product
        {
            Name = "Other",
            ean = "222",
            price = 10,
            stock = 5,
            sku = "ELEC-001",
            category = new Category { Name = "Electronics" },
            created_by = 1,
            updated_by = 1
        });

        var books = (await service.GetByCategoryAsync("Books")).ToList();
        Assert.Single(books);
        Assert.Equal("Book 1", books[0].Name);
    }
}
