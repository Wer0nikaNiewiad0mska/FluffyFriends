using EShop.Application.Interfaces;
using EShop.Domain.Models;
using EShop.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Application;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repository;

    public CategoryService(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public Task<IEnumerable<Category>> GetAllAsync() => _repository.GetAllAsync();
    public Task<Category?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);
    public Task AddAsync(Category category) => _repository.AddAsync(category);
    public Task UpdateAsync(Category category) => _repository.UpdateAsync(category);
    public Task DeleteAsync(int id) => _repository.DeleteAsync(id);
}
