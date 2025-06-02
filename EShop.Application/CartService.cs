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

    public Task<Cart?> GetCartAsync(Guid userId)
        => _cartRepository.GetByUserIdAsync(userId);

    public Task AddItemAsync(Guid userId, int productId, int quantity)
        => _cartRepository.AddOrUpdateItemAsync(userId, productId, quantity);

    public Task RemoveItemAsync(Guid userId, int productId)
        => _cartRepository.RemoveItemAsync(userId, productId);

    public Task ClearCartAsync(Guid userId)
        => _cartRepository.ClearCartAsync(userId);
}
