using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using User.Application.Services.Interfaces;
using User.Domain.Models;
using User.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;


namespace UserService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly DataContext _context;
    private readonly IPasswordHasher<User_> _hasher;

    public AccountController(DataContext context, IPasswordHasher<User_> hasher)
    {
        _context = context;
        _hasher = hasher;
    }

    [HttpGet("account details")]
    [Authorize]
    public IActionResult GetUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var username = User.FindFirstValue(ClaimTypes.Name);
        var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
        var history = new[]
        {
            new {
                orderId = 1,
                orderDate = DateTime.UtcNow,
                products = new[] {
                    new { productId = 0, productyName = "Fluffy friend", quantity = 2 },
                },
                orderPrice = 100
            }
        };

        return Ok(new { username, roles, orderHistory = history });
    }

    public class ChangePasswordRequest
    {
        public string OldPassword { get; set; } = default!;
        public string NewPassword { get; set; } = default!;
    }

    [HttpPost("change password")]

    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest req)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            return NotFound();

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, req.OldPassword);
        if (result != PasswordVerificationResult.Success)
            return BadRequest("Wrong old password.");

        user.PasswordHash = _hasher.HashPassword(user, req.NewPassword);
        await _context.SaveChangesAsync();

        return Ok("Password has been changed.");
    }


    [HttpGet("admin data (products/clients)")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetAdminData([FromQuery] string type)
    {
        if (string.IsNullOrEmpty(type))
            return BadRequest("Musisz podać parametr 'type' (products lub clients).");

        if (type == "products")
        {
            var products = new[]
            {
            new { Id = 1, Name = "Fluffy Bear", Quantity = 10 },
            new { Id = 2, Name = "Soft Cat", Quantity = 5 },
        };

            return Ok(products);
        }
        else if (type == "clients")
        {
            var clients = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Where(u => u.UserRoles.Any(ur => ur.Role.Name == "Client"))
                .Select(u => new
                {
                    u.Username,
                    u.Email,
                    u.CreatedAt
                })
                .ToListAsync();

            return Ok(clients);
        }

        return BadRequest("Unknown data type. Use 'products' or 'clients'.");
    }
}
