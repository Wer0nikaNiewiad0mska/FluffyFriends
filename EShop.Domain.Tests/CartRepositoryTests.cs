using EShop.Domain.Models;
using EShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Domain.Tests;

public class CartRepositoryTests : RepositoryTestBase
{
    private CartRepository _cartRepository;
    private Category _testCategory;

    public CartRepositoryTests() : base("TestDatabase_CartRepository")
    {
        _cartRepository = new CartRepository(_context);

        _testCategory = new Category { Name = "Test Category", created_by = 0, updated_by = 0 };
        _context.Categories.Add(_testCategory);
        _context.SaveChanges();

        _context.Products.Add(new Product
        {
            Name = "Test Product 1",
            ean = "123",
            price = 10m,
            stock = 100,
            sku = "TP001",
            category = _testCategory,
            created_by = 0,
            updated_by = 0
        });
        _context.Products.Add(new Product
        {
            Name = "Test Product 2",
            ean = "456",
            price = 20m,
            stock = 50,
            sku = "TP002",
            category = _testCategory,
            created_by = 0,
            updated_by = 0
        });
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsCartWithItems()
    {
        var userId = 1;
        var product1 = await _context.Products.FirstAsync(p => p.Name == "Test Product 1");
        var product2 = await _context.Products.FirstAsync(p => p.Name == "Test Product 2");

        var cart = new Cart { UserId = userId, created_by = userId, updated_by = userId };
        cart.Items.Add(new CartItem { ProductId = product1.id, Quantity = 2, created_by = userId, updated_by = userId });
        cart.Items.Add(new CartItem { ProductId = product2.id, Quantity = 1, created_by = userId, updated_by = userId });
        _context.Carts.Add(cart);
        await _context.SaveChangesAsync();

        var retrievedCart = await _cartRepository.GetByUserIdAsync(userId);

        Assert.NotNull(retrievedCart);
        Assert.Equal(userId, retrievedCart.UserId);
        Assert.Equal(2, retrievedCart.Items.Count);
        Assert.Contains(retrievedCart.Items, i => i.ProductId == product1.id && i.Quantity == 2);
        Assert.Contains(retrievedCart.Items, i => i.ProductId == product2.id && i.Quantity == 1);
        Assert.All(retrievedCart.Items, i => Assert.NotNull(i.Product));
    }

    [Fact]
    public async Task GetByUserIdAsync_ReturnsNullForNonExistingCart()
    {
        var retrievedCart = await _cartRepository.GetByUserIdAsync(999);

        Assert.Null(retrievedCart);
    }

    [Fact]
    public async Task AddOrUpdateItemAsync_AddsNewCartAndItem()
    {
        var userId = 1;
        var productId = (await _context.Products.FirstAsync()).id;
        var quantity = 5;

        await _cartRepository.AddOrUpdateItemAsync(userId, productId, quantity);

        var cart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
        Assert.NotNull(cart);
        Assert.Single(cart.Items);
        Assert.Equal(productId, cart.Items.First().ProductId);
        Assert.Equal(quantity, cart.Items.First().Quantity);
    }

    [Fact]
    public async Task AddOrUpdateItemAsync_UpdatesExistingItemQuantity()
    {
        var userId = 1;
        var product = await _context.Products.FirstAsync();
        var productId = product.id;
        var initialQuantity = 2;
        var additionalQuantity = 3;

        var cart = new Cart { UserId = userId, created_by = userId, updated_by = userId };
        cart.Items.Add(new CartItem { ProductId = productId, Quantity = initialQuantity, created_by = userId, updated_by = userId });
        _context.Carts.Add(cart);
        await _context.SaveChangesAsync();

        await _cartRepository.AddOrUpdateItemAsync(userId, productId, additionalQuantity);

        var updatedCart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
        Assert.NotNull(updatedCart);
        Assert.Single(updatedCart.Items);
        Assert.Equal(productId, updatedCart.Items.First().ProductId);
        Assert.Equal(initialQuantity + additionalQuantity, updatedCart.Items.First().Quantity);
    }

    [Fact]
    public async Task AddOrUpdateItemAsync_AddsNewItemToExistingCart()
    {
        var userId = 1;
        var product1 = await _context.Products.FirstAsync(p => p.Name == "Test Product 1");
        var product2 = await _context.Products.FirstAsync(p => p.Name == "Test Product 2");
        var quantity1 = 2;
        var quantity2 = 1;

        var cart = new Cart { UserId = userId, created_by = userId, updated_by = userId };
        cart.Items.Add(new CartItem { ProductId = product1.id, Quantity = quantity1, created_by = userId, updated_by = userId });
        _context.Carts.Add(cart);
        await _context.SaveChangesAsync();

        await _cartRepository.AddOrUpdateItemAsync(userId, product2.id, quantity2);

        var updatedCart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
        Assert.NotNull(updatedCart);
        Assert.Equal(2, updatedCart.Items.Count);
        Assert.Contains(updatedCart.Items, i => i.ProductId == product1.id && i.Quantity == quantity1);
        Assert.Contains(updatedCart.Items, i => i.ProductId == product2.id && i.Quantity == quantity2);
    }

    [Fact]
    public async Task RemoveItemAsync_RemovesExistingItem()
    {
        var userId = 1;
        var product1 = await _context.Products.FirstAsync(p => p.Name == "Test Product 1");
        var product2 = await _context.Products.FirstAsync(p => p.Name == "Test Product 2");

        var cart = new Cart { UserId = userId, created_by = userId, updated_by = userId };
        cart.Items.Add(new CartItem { ProductId = product1.id, Quantity = 2, created_by = userId, updated_by = userId });
        cart.Items.Add(new CartItem { ProductId = product2.id, Quantity = 1, created_by = userId, updated_by = userId });
        _context.Carts.Add(cart);
        await _context.SaveChangesAsync();

        await _cartRepository.RemoveItemAsync(userId, product1.id);

        var updatedCart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
        Assert.NotNull(updatedCart);
        Assert.Single(updatedCart.Items);
        Assert.DoesNotContain(updatedCart.Items, i => i.ProductId == product1.id);
        Assert.Contains(updatedCart.Items, i => i.ProductId == product2.id && i.Quantity == 1);
    }

    [Fact]
    public async Task RemoveItemAsync_DoesNothingIfItemNotFound()
    {
        var userId = 1;
        var product1 = await _context.Products.FirstAsync(p => p.Name == "Test Product 1");

        var cart = new Cart { UserId = userId, created_by = userId, updated_by = userId };
        cart.Items.Add(new CartItem { ProductId = product1.id, Quantity = 2, created_by = userId, updated_by = userId });
        _context.Carts.Add(cart);
        await _context.SaveChangesAsync();

        await _cartRepository.RemoveItemAsync(userId, 999);

        var updatedCart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
        Assert.NotNull(updatedCart);
        Assert.Single(updatedCart.Items);
    }

    [Fact]
    public async Task ClearCartAsync_RemovesAllItemsFromCart()
    {
        var userId = 1;
        var product1 = await _context.Products.FirstAsync(p => p.Name == "Test Product 1");
        var product2 = await _context.Products.FirstAsync(p => p.Name == "Test Product 2");

        var cart = new Cart { UserId = userId, created_by = userId, updated_by = userId };
        cart.Items.Add(new CartItem { ProductId = product1.id, Quantity = 2, created_by = userId, updated_by = userId });
        cart.Items.Add(new CartItem { ProductId = product2.id, Quantity = 1, created_by = userId, updated_by = userId });
        _context.Carts.Add(cart);
        await _context.SaveChangesAsync();

        await _cartRepository.ClearCartAsync(userId);

        var clearedCart = await _context.Carts.Include(c => c.Items).FirstOrDefaultAsync(c => c.UserId == userId);
        Assert.NotNull(clearedCart);
        Assert.Empty(clearedCart.Items);
    }

    [Fact]
    public async Task ClearCartAsync_DoesNothingIfCartNotFound()
    {
        await _cartRepository.ClearCartAsync(999);

        
        var cartCount = await _context.Carts.CountAsync();
        Assert.Equal(0, cartCount);
    }
}