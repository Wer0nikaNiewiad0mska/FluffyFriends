using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.RegularExpressions;
using User.Application.Services.Interfaces;
using User.Domain.CustomExceptions;
using User.Domain.Models;
using User.Domain.Repositories;


namespace UserService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly ILoginService _loginService;
    private readonly DataContext _context;
    private readonly IPasswordHasher<User_> _hasher;

    public LoginController(ILoginService loginservice, DataContext context, IPasswordHasher<User_> hasher)
    {
        _loginService = loginservice;
        _context = context;
        _hasher = hasher;
    }

    [HttpPost]
    public IActionResult Login([FromBody] User.Domain.Models.LoginRequest req)
    {
        try
        {
            var token = _loginService.Login(req.Username, req.Password);
            return Ok(new { token });
        }
        catch (InvalidCredentialsException) { return Unauthorized("Invalid login data."); }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] User.Domain.Models.RegisterRequest req)
    {
        //if (!Regex.IsMatch(req.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
          //  return BadRequest("Not real email address.");

        var existing = await _context.Users.FirstOrDefaultAsync(u => u.Username == req.Username);
        if (existing != null)
            return Conflict("Username already exists.");

        var clientRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "Client");
        if (clientRole == null)
            return StatusCode(500, "Role not found");

        var user = new User_
        {
            Username = req.Username,
            Email = req.Email,
            PasswordHash = _hasher.HashPassword(null, req.Password),
            UserRoles = new List<UserRole>
            {
                new UserRole { Role = clientRole }
            }
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok("Account created.");
    }
   
    [HttpGet("admin")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult AdminOnly()
    {
        return Ok("Admin access");
    }
    
    [HttpGet("employee")]
    [Authorize(Policy = "EmployeeOnly")]
    public IActionResult EmployeeOnly()
    {
        return Ok("Employee access.");
    }
}
