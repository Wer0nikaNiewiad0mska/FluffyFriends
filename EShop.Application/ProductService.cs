using EShop.Application.Interfaces;
using EShop.Domain.Models;
using EShop.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Application;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;

    public ProductService(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
        => await _productRepository.GetAllAsync();

    public async Task<Product?> GetByIdAsync(int id)
        => await _productRepository.GetByIdAsync(id);

    public async Task AddAsync(Product product)
        => await _productRepository.AddAsync(product);

    public async Task UpdateAsync(Product product)
        => await _productRepository.UpdateAsync(product);

    public async Task DeleteAsync(int id)
        => await _productRepository.DeleteAsync(id);

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string categoryName)
        => await _productRepository.GetByCategoryAsync(categoryName);

    public async Task<bool> ExistsAsync(int id)
        => await _productRepository.ExistsAsync(id);

    public async Task<int> CountAsync()
        => await _productRepository.CountAsync();
}
