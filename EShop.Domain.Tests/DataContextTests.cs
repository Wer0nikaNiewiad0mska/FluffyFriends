using Microsoft.EntityFrameworkCore;
using EShop.Domain.Repositories;
using EShop.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Domain.Tests;

public class DataContextTests : RepositoryTestBase
{
    public DataContextTests() : base("TestDatabase_DataContext") { }


    [Fact]
    public async Task SaveChangesAsync_SetsDeletedToFalseForNewEntities()
    {
        var category = new Category { Name = "New Category", created_by = 1, updated_by = 1 };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        Assert.False(category.deleted);
    }
}