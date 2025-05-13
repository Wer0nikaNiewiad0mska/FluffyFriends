using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using User.Application.Services;
using User.Domain.CustomExceptions;
using User.Domain.Models;

namespace UserService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LoginController : ControllerBase
{
    protected ILoginService _loginService;

    public LoginController(ILoginService loginservice)
    {
        _loginService = loginservice;
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

    [HttpGet]
    [Authorize]
    [Authorize(Policy = "AdminOnly")]
    public IActionResult AdminPage()
    {
        return Ok();
    }
}
