using EShop.Domain.Models;
using EShop.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Domain.Seeders;

public class EShopSeeder(DataContext context) : IEShopSeeder
{
    public async Task Seed()
    {
        if (!context.Categories.Any())
        {
            var categories = new List<Category>
            {
                new Category { Name = "Plushies", created_by = 0, updated_by = 0 },
                new Category { Name = "Keychains", created_by = 0, updated_by = 0 },
                new Category { Name = "Stationery", created_by = 0, updated_by = 0 }
            };

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync();
        }

        if (!context.Products.Any())
        {
            var plushies = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Plushies");
            var keychains = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Keychains");
            var stationery = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Stationery");

            var products = new List<Product>
            {
                new Product
                {
                    Name = "Adaś the Shark",
                    ean = "1001001001001",
                    price = 59.99m,
                    stock = 30,
                    sku = "PLSH-SHARK-001",
                    category = plushies!,
                    created_by = 0,
                    updated_by = 0,
                },
                new Product
                {
                    Name = "Kiki the Hamster",
                    ean = "1001001001002",
                    price = 49.99m,
                    stock = 40,
                    sku = "PLSH-HAMSTER-001",
                    category = plushies!,
                    created_by = 0,
                    updated_by = 0,
                },
                new Product
                {
                    Name = "Cute Keychain",
                    ean = "1001001001003",
                    price = 19.99m,
                    stock = 100,
                    sku = "KEYC-CUTE-001",
                    category = keychains!,
                    created_by = 0,
                    updated_by = 0,
                },
                new Product
                {
                    Name = "Funny Notebook",
                    ean = "1001001001004",
                    price = 14.99m,
                    stock = 80,
                    sku = "STAT-NOTE-001",
                    category = stationery!,
                    created_by = 0,
                    updated_by = 0,
                }
            };

            context.Products.AddRange(products);
            await context.SaveChangesAsync();
        }
    }
}