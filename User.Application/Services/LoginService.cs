using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using User.Domain.CustomExceptions;
using User.Application.Services;

namespace User.Application.Services;

public class LoginService : ILoginService
{
    protected IJwtTokenService _jwtTokenService;

    public LoginService(IJwtTokenService jwtTokenService) { _jwtTokenService = jwtTokenService; }

    public string Login(string username, string password)
    {
        if (username == "admin" && password == "password")
        {
            var roles = new List<string> { "Client", "Employee", "Administrator" };
            var token = _jwtTokenService.GenerateTOken(123, roles);
            return token;
        }
        else
        {
            throw new InvalidCredentialsException();
        }
    }
}
