using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using User.Application.Services;
using User.Domain.CustomExceptions;
using User.Domain.Models;

namespace UserService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    private readonly ILoginService _loginService;
    protected readonly IUserRepository _userRepository;

    public LoginController(ILoginService loginservice, IUserRepository userRepository)
    {
        _loginService = loginservice;
        _userRepository = userRepository;
    }

    [HttpPost]
    public IActionResult Login([FromBody] User.Domain.Models.LoginRequest req)
    {
        try
        {
            var token = _loginService.Login(req.Username, req.Password);
            return Ok(new { token });
        }
        catch (InvalidCredentialsException) { return Unauthorized(); }
    }

    [HttpPost("register")]
    public IActionResult Register([FromBody] User.Domain.Models.LoginRequest req)
    {
        var existing = _userRepository.GetByUsername(req.Username);
        if (existing != null)
            return Conflict("Username already exists");

        var user = new UserAccount
        {
            Username = req.Username,
            Password = req.Password,
            Role = "Client"
        };

        _userRepository.Add(user);
        return Ok();
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult GetUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var username = User.FindFirstValue(ClaimTypes.Name);
        var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();

        return Ok(new { userId, username, roles });
    }

    [HttpGet("admin")]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult AdminOnly()
    {
        return Ok("Dostęp dla administratora");
    }

    [HttpGet("employee")]
    [Authorize(Policy = "EmployeeOnly")]
    public IActionResult EmployeeOnly()
    {
        return Ok("Dostęp dla pracownika");
    }
}
