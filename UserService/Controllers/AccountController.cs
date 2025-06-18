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
                orderId = "X",
                orderDate = DateTime.UtcNow,
                products = new[] {
                    new { productId = "X", productName = "product_name", quantity = "X" },
                },
                orderPrice = "XX.XX"
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

    [HttpPost("change email")]
    [Authorize]
    public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailRequest req)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdString, out var userId))
        {
            return Unauthorized("Invalid user ID in token.");
        }

        var user = await _context.Users.FindAsync(userId);

        if (user == null)
            return NotFound("User not found.");

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, req.Password);
        if (result != PasswordVerificationResult.Success)
            return BadRequest("Incorrect password.");

        var existingUserWithEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == req.NewEmail && u.Id != userId);
        if (existingUserWithEmail != null)
            return Conflict("Email address already in use.");

        user.Email = req.NewEmail;
        await _context.SaveChangesAsync();

        return Ok("Email address has been changed.");
    }

    [HttpGet("admin data (employees/clients)")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> GetAdminData([FromQuery] string type)
    {
        if (string.IsNullOrEmpty(type))
            return BadRequest("You need to put one of these parameters: (products / clients).");

        if (type == "employees")
        {
            var employees = await _context.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .Where(u => u.UserRoles.Any(ur => ur.Role.Name == "Employee"))
                .Select(u => new
                {
                    u.Username,
                    u.Email,
                    u.CreatedAt
                })
                .ToListAsync();

            return Ok(employees);
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

        return BadRequest("Unknown data type. Use 'employees' or 'clients'.");
    }

    [HttpDelete("clients/{clientId}")]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> DeleteClient(int clientId)
    {
        var clientToDelete = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == clientId);

        if (clientToDelete == null)
            return NotFound("Client not found.");

        var isClient = clientToDelete.UserRoles.Any(ur => ur.Role != null && ur.Role.Name == "Client");

        if (!isClient)
            return BadRequest("Cannot delete: the specified user is not a client or does not have a properly associated client role.");

        _context.Users.Remove(clientToDelete);
        await _context.SaveChangesAsync();

        return Ok($"Client with ID {clientId} has been deleted.");
    }
}
