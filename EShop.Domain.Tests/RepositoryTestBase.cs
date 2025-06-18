using Microsoft.EntityFrameworkCore;
using EShop.Domain.Repositories;
using EShop.Domain.Models;
using EShop.Domain.Seeders;

namespace EShop.Domain.Tests;

public abstract class RepositoryTestBase : IDisposable
{
    protected DataContext _context;

    protected RepositoryTestBase(string databaseName)
    {
        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: databaseName)
            .Options;
        _context = new DataContext(options);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}