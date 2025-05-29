using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using User.Domain.Models;

namespace User.Domain.Repositories;

public class DataContext :DbContext
{
    public DbSet<User_> Users => Set<User_>();
    public DbSet<Role> Roles => Set<Role>();

    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>().HasData(
            new Role { Id = 1, Name = "Administrator" },
            new Role { Id = 2, Name = "Employee" },
            new Role { Id = 3, Name = "Client" }
        );

        modelBuilder.Entity<User_>().HasData(
            new User_
            {
                Id = 1,
                Username = "admin",
                Email = "admin@example.com",
                PasswordHash = "admin123",
                IsActive = true
            }
        );
    }
}
