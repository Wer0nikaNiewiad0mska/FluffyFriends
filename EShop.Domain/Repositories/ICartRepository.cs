using EShop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Domain.Repositories;

public interface ICartRepository
{
    Task<Cart?> GetByUserIdAsync(Guid userId);
    Task AddOrUpdateItemAsync(Guid userId, int productId, int quantity);
    Task RemoveItemAsync(Guid userId, int productId);
    Task ClearCartAsync(Guid userId);
}
