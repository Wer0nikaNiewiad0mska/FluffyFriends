using EShop.Domain.Repositories;
using EShop.Domain.Models;
using EShop.Domain.Seeders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Domain.Tests;

public class CategoryRepositoryTests : RepositoryTestBase
{
    private CategoryRepository _categoryRepository;

    public CategoryRepositoryTests() : base("TestDatabase_CategoryRepository")
    {
        _categoryRepository = new CategoryRepository(_context);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllNonDeletedCategories()
    {
        _context.Categories.Add(new Category { Name = "Category 1", created_by = 0, updated_by = 0 });
        _context.Categories.Add(new Category { Name = "Category 2", created_by = 0, updated_by = 0 });
        _context.Categories.Add(new Category { Name = "Deleted Category", deleted = true, created_by = 0, updated_by = 0 });
        await _context.SaveChangesAsync();

        var categories = await _categoryRepository.GetAllAsync();

        Assert.Equal(2, categories.Count());
        Assert.Contains(categories, c => c.Name == "Category 1");
        Assert.Contains(categories, c => c.Name == "Category 2");
        Assert.DoesNotContain(categories, c => c.Name == "Deleted Category");
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsCategoryWhenExistsAndNotDeleted()
    {
        var category = new Category { Name = "Test Category", created_by = 0, updated_by = 0 };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        var retrievedCategory = await _categoryRepository.GetByIdAsync(category.id);

        Assert.NotNull(retrievedCategory);
        Assert.Equal("Test Category", retrievedCategory.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNullWhenCategoryIsDeleted()
    {
        var category = new Category { Name = "Deleted Category", deleted = true, created_by = 0, updated_by = 0 };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        var retrievedCategory = await _categoryRepository.GetByIdAsync(category.id);

        Assert.Null(retrievedCategory);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNullWhenNotExists()
    {
        var retrievedCategory = await _categoryRepository.GetByIdAsync(999);

        Assert.Null(retrievedCategory);
    }

    [Fact]
    public async Task AddAsync_AddsNewCategory()
    {
        var category = new Category { Name = "New Category", created_by = 0, updated_by = 0 };

        await _categoryRepository.AddAsync(category);

        Assert.Equal(1, await _context.Categories.CountAsync());
        Assert.Equal("New Category", _context.Categories.First().Name);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingCategory()
    {
        var category = new Category { Name = "Original Name", created_by = 0, updated_by = 0 };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        category.Name = "Updated Name";

        await _categoryRepository.UpdateAsync(category);

        var updatedCategory = await _context.Categories.FindAsync(category.id);
        Assert.Equal("Updated Name", updatedCategory.Name);
    }

    [Fact]
    public async Task DeleteAsync_SetsCategoryAsDeleted()
    {
        var category = new Category { Name = "Category to Delete", created_by = 0, updated_by = 0 };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        await _categoryRepository.DeleteAsync(category.id);

        var deletedCategory = await _context.Categories.FindAsync(category.id);
        Assert.True(deletedCategory.deleted);

        var categories = await _categoryRepository.GetAllAsync();
        Assert.DoesNotContain(categories, c => c.id == category.id);
    }

    [Fact]
    public async Task DeleteAsync_DoesNothingIfCategoryNotFound()
    {
        await _categoryRepository.DeleteAsync(999);

        var count = await _context.Categories.CountAsync();
        Assert.Equal(0, count);
    }
}