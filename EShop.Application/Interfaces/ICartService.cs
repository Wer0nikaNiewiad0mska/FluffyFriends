using EShop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Application.Interfaces;

public interface ICartService
{
    Task<Cart?> GetCartAsync(Guid userId);
    Task AddItemAsync(Guid userId, int productId, int quantity);
    Task RemoveItemAsync(Guid userId, int productId);
    Task ClearCartAsync(Guid userId);
}
