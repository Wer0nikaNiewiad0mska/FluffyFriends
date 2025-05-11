using Microsoft.EntityFrameworkCore;
using EShop.Domain.Models;
using Microsoft.Extensions.Options;

namespace EShop.Domain.Repositories;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options) { }

    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        if (!options.IsConfigured)
        {
            options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=EShopDb;Trusted_Connection=True;");
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var entries = ChangeTracker
            .Entries()
            .Where(e => e.Entity is BaseModel &&
                        (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entry in entries)
        {
            var model = (BaseModel)entry.Entity;
            model.updated_at = DateTime.UtcNow;

            if (entry.State == EntityState.Added)
            {
                model.created_at = DateTime.UtcNow;
                model.deleted = false;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }
}
