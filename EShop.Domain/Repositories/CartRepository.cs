using EShop.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Domain.Repositories;

public class CartRepository : ICartRepository
{
    private readonly DataContext _context;

    public CartRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<Cart?> GetByUserIdAsync(int userId)
    {
        return await _context.Carts
            .Include(c => c.Items)
            .ThenInclude(i => i.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId && !c.deleted);
    }

    public async Task AddOrUpdateItemAsync(int userId, int productId, int quantity)
    {
        var cart = await GetByUserIdAsync(userId);
        var now = DateTime.UtcNow;

        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId,
                created_at = now,
                updated_at = now,
                created_by = userId,
                updated_by = userId,
            };
            await _context.Carts.AddAsync(cart);
        }

        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId && !i.deleted);

        if (item != null)
        {
            item.Quantity += quantity;
            item.updated_at = now;
            item.updated_by = userId;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                ProductId = productId,
                Quantity = quantity,
                created_at = now,
                updated_at = now,
                created_by = userId,
                updated_by = userId,
            });
        }

        cart.updated_at = now;
        cart.updated_by = userId;

        await _context.SaveChangesAsync();
    }

    public async Task RemoveItemAsync(int userId, int productId)
    {
        var cart = await GetByUserIdAsync(userId);
        var item = cart?.Items.FirstOrDefault(i => i.ProductId == productId && !i.deleted);

        if (item != null)
        {
            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }

    public async Task ClearCartAsync(int userId)
    {
        var cart = await GetByUserIdAsync(userId);
        if (cart != null)
        {
            _context.CartItems.RemoveRange(cart.Items);
            await _context.SaveChangesAsync();
        }
    }
}
