using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using User.Application.Services;
using User.Application.Services.Interfaces;
using User.Domain.CustomExceptions;
using User.Domain.Models;
using User.Domain.Repositories;

namespace User.Application.Tests;

public class LoginServiceTests
{
    private readonly Mock<IJwtTokenService> _mockJwtTokenService;
    private DataContext _context;
    private LoginService _loginService;
    private readonly PasswordHasher<User_> _hasher;

    public LoginServiceTests()
    {
        _mockJwtTokenService = new Mock<IJwtTokenService>();

        var options = new DbContextOptionsBuilder<DataContext>()
            .UseInMemoryDatabase(databaseName: "TestDbForLoginService")
            .Options;
        _context = new DataContext(options);
        _context.Database.EnsureDeleted();
        _context.Database.EnsureCreated();

        _hasher = new PasswordHasher<User_>();

        _loginService = new LoginService(_mockJwtTokenService.Object, _context);
    }

    [Fact]
    public void Login_WithValidCredentials_ReturnsToken()
    {
        var password = "TestPassword123!";
        var hashedPassword = _hasher.HashPassword(null, password);

        var clientRole = new Role { Id = 1, Name = "Client" };
        var adminRole = new Role { Id = 3, Name = "Admin" };

        var user = new User_
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = hashedPassword,
            UserRoles = new List<UserRole>
                {
                    new UserRole { User = null!, Role = clientRole, UserId = 1, RoleId = clientRole.Id },
                    new UserRole { User = null!, Role = adminRole, UserId = 1, RoleId = adminRole.Id }
                }
        };
        _context.Users.Add(user);
        _context.SaveChanges();

        var expectedToken = "mocked_jwt_token";
        _mockJwtTokenService.Setup(s => s.GenerateTOken(
            It.IsAny<int>(), It.IsAny<string>(), It.IsAny<List<string>>()
        )).Returns(expectedToken);

        var token = _loginService.Login("testuser", password);

        Assert.Equal(expectedToken, token);
        _mockJwtTokenService.Verify(s => s.GenerateTOken(
            user.Id,
            user.Username,
            It.Is<List<string>>(roles => roles.Contains("Client") && roles.Contains("Admin") && roles.Count == 2)
        ), Times.Once);
    }

    [Fact]
    public void Login_WithInvalidUsername_ThrowsInvalidCredentialsException()
    {
        Assert.Throws<InvalidCredentialsException>(() => _loginService.Login("nonexistentuser", "password"));
    }

    [Fact]
    public void Login_WithInvalidPassword_ThrowsInvalidCredentialsException()
    {
        var password = "TestPassword123!";
        var hashedPassword = _hasher.HashPassword(null, password);

        var user = new User_
        {
            Id = 1,
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = hashedPassword,
            UserRoles = new List<UserRole>()
        };
        _context.Users.Add(user);
        _context.SaveChanges();

        Assert.Throws<InvalidCredentialsException>(() => _loginService.Login("testuser", "wrongpassword"));
    }

    

    public void Dispose()
    {
        _context.Dispose();
    }
}
