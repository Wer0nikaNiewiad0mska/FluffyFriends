using EShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using EShop.Domain.Models;

namespace EShop.Domain.Tests;

public class ProductRepositoryTests : RepositoryTestBase
{
    private ProductRepository _productRepository;
    private Category _testCategory;

    public ProductRepositoryTests() : base("TestDatabase_ProductRepository")
    {
        _productRepository = new ProductRepository(_context);

        _testCategory = new Category { Name = "Electronics", created_by = 0, updated_by = 0 };
        _context.Categories.Add(_testCategory);
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllProductsWithCategory()
    {
        _context.Products.Add(new Product { Name = "Laptop", ean = "1", price = 1200m, stock = 10, sku = "LPT001", category = _testCategory, created_by = 0, updated_by = 0 });
        _context.Products.Add(new Product { Name = "Mouse", ean = "2", price = 25m, stock = 50, sku = "MSE001", category = _testCategory, created_by = 0, updated_by = 0 });
        await _context.SaveChangesAsync();

        var products = await _productRepository.GetAllAsync();

        Assert.Equal(2, products.Count());
        Assert.All(products, p => Assert.NotNull(p.category));
        Assert.Contains(products, p => p.Name == "Laptop" && p.category.Name == "Electronics");
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsProductWithCategoryWhenExists()
    {
        var product = new Product { Name = "Keyboard", ean = "3", price = 75m, stock = 30, sku = "KYB001", category = _testCategory, created_by = 0, updated_by = 0 };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var retrievedProduct = await _productRepository.GetByIdAsync(product.id);

        Assert.NotNull(retrievedProduct);
        Assert.Equal("Keyboard", retrievedProduct.Name);
        Assert.NotNull(retrievedProduct.category);
        Assert.Equal("Electronics", retrievedProduct.category.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNullWhenNotExists()
    {
        var retrievedProduct = await _productRepository.GetByIdAsync(999);

        Assert.Null(retrievedProduct);
    }

    [Fact]
    public async Task AddAsync_AddsNewProduct()
    {
        var product = new Product { Name = "Webcam", ean = "4", price = 50m, stock = 20, sku = "WBCM001", category = _testCategory, created_by = 0, updated_by = 0 };

        await _productRepository.AddAsync(product);

        Assert.Equal(1, await _context.Products.CountAsync());
        Assert.Equal("Webcam", _context.Products.First().Name);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingProduct()
    {
        var product = new Product { Name = "Old Name", ean = "5", price = 10m, stock = 5, sku = "OLD001", category = _testCategory, created_by = 0, updated_by = 0 };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        product.Name = "New Name";
        product.price = 15m;

        await _productRepository.UpdateAsync(product);

        var updatedProduct = await _context.Products.FindAsync(product.id);
        Assert.Equal("New Name", updatedProduct.Name);
        Assert.Equal(15m, updatedProduct.price);
    }

    [Fact]
    public async Task DeleteAsync_RemovesProduct()
    {
        var product = new Product { Name = "Product to Delete", ean = "6", price = 100m, stock = 10, sku = "DEL001", category = _testCategory, created_by = 0, updated_by = 0 };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        await _productRepository.DeleteAsync(product.id);

        Assert.Equal(0, await _context.Products.CountAsync());
        Assert.Null(await _productRepository.GetByIdAsync(product.id));
    }

    [Fact]
    public async Task DeleteAsync_DoesNothingIfProductNotFound()
    {
        _context.Products.Add(new Product { Name = "Existing Product", ean = "7", price = 10m, stock = 1, sku = "EXS001", category = _testCategory, created_by = 0, updated_by = 0 });
        await _context.SaveChangesAsync();
        var initialCount = await _context.Products.CountAsync();

        await _productRepository.DeleteAsync(999);

        Assert.Equal(initialCount, await _context.Products.CountAsync());
    }

    [Fact]
    public async Task GetByCategoryAsync_ReturnsProductsInSpecifiedCategory()
    {
        var category1 = _testCategory;
        var category2 = new Category { Name = "Books", created_by = 0, updated_by = 0 };
        _context.Categories.Add(category2);
        _context.SaveChanges();

        _context.Products.Add(new Product { Name = "E-Reader", ean = "E1", price = 200m, stock = 5, sku = "EREAD001", category = category1, created_by = 0, updated_by = 0 });
        _context.Products.Add(new Product { Name = "Novel", ean = "N1", price = 15m, stock = 20, sku = "NOVEL001", category = category2, created_by = 0, updated_by = 0 });
        _context.Products.Add(new Product { Name = "Monitor", ean = "M1", price = 300m, stock = 8, sku = "MONIT001", category = category1, created_by = 0, updated_by = 0 });
        await _context.SaveChangesAsync();

        var electronicsProducts = await _productRepository.GetByCategoryAsync("Electronics");
        var bookProducts = await _productRepository.GetByCategoryAsync("Books");
        var nonExistentCategoryProducts = await _productRepository.GetByCategoryAsync("Clothes");


        Assert.Equal(2, electronicsProducts.Count());
        Assert.Contains(electronicsProducts, p => p.Name == "E-Reader");
        Assert.Contains(electronicsProducts, p => p.Name == "Monitor");
        Assert.DoesNotContain(electronicsProducts, p => p.Name == "Novel");

        Assert.Single(bookProducts);
        Assert.Contains(bookProducts, p => p.Name == "Novel");

        Assert.Empty(nonExistentCategoryProducts);
    }

    [Fact]
    public async Task ExistsAsync_ReturnsTrueIfProductExists()
    {
        var product = new Product { Name = "Existing Prod", ean = "8", price = 1m, stock = 1, sku = "EXST001", category = _testCategory, created_by = 0, updated_by = 0 };
        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        var exists = await _productRepository.ExistsAsync(product.id);
        var notExists = await _productRepository.ExistsAsync(999);

        Assert.True(exists);
        Assert.False(notExists);
    }

    [Fact]
    public async Task CountAsync_ReturnsCorrectProductCount()
    {
        _context.Products.Add(new Product { Name = "Prod A", ean = "A", price = 1m, stock = 1, sku = "A001", category = _testCategory, created_by = 0, updated_by = 0 });
        _context.Products.Add(new Product { Name = "Prod B", ean = "B", price = 1m, stock = 1, sku = "B001", category = _testCategory, created_by = 0, updated_by = 0 });
        await _context.SaveChangesAsync();

        var count = await _productRepository.CountAsync();

        Assert.Equal(2, count);
    }

    [Fact]
    public async Task CountAsync_ReturnsZeroForEmptyTable()
    {
        var count = await _productRepository.CountAsync();

        Assert.Equal(0, count);
    }
}