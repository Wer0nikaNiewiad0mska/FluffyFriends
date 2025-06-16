using EShop.Application.Interfaces;
using EShop.Domain.Models;
using EShop.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Application;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;

    public CartService(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public Task<Cart?> GetCartAsync(int userId)
        => _cartRepository.GetByUserIdAsync(userId);

    public Task AddItemAsync(int userId, int productId, int quantity)
        => _cartRepository.AddOrUpdateItemAsync(userId, productId, quantity);

    public Task RemoveItemAsync(int userId, int productId)
        => _cartRepository.RemoveItemAsync(userId, productId);

    public Task ClearCartAsync(int userId)
        => _cartRepository.ClearCartAsync(userId);
}
