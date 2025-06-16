using EShop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Domain.Repositories;

public interface ICartRepository
{
    Task<Cart?> GetByUserIdAsync(int userId);
    Task AddOrUpdateItemAsync(int userId, int productId, int quantity);
    Task RemoveItemAsync(int userId, int productId);
    Task ClearCartAsync(int userId);
}
