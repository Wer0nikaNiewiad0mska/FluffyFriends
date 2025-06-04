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
                idZamowienia = 1,
                dataZamowienia = DateTime.UtcNow,
                produkty = new[] {
                    new { idProduktu = 101, nazwaProduktu = "Karma dla psa", ilość = 2 },
                    new { idProduktu = 102, nazwaProduktu = "Piłka", ilość = 1 }
                }
            }
        };

        return Ok(new { username, roles, historiaZamowien = history });
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

        if (user.PasswordHash != req.OldPassword)
            return BadRequest("Niepoprawne stare hasło.");

        user.PasswordHash = req.NewPassword;
        await _context.SaveChangesAsync();

        return Ok("Hasło zostało zmienione.");
    }
}
