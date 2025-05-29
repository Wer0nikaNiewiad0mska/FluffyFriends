using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using User.Domain.CustomExceptions;
using User.Domain.Models;
using User.Application.Services;

namespace User.Application.Services;

public class LoginService : ILoginService
{
    protected IJwtTokenService _jwtTokenService;
    private readonly IUserRepository _userRepository;

    public LoginService(IJwtTokenService jwtTokenService, IUserRepository userRepository)
    {
        _jwtTokenService = jwtTokenService;
        _userRepository = userRepository;
    }

    public string Login(string username, string password)
    {
        var user = _userRepository.GetByUsername(username);
        if (user == null || user.Password != password)
            throw new InvalidCredentialsException();

        var token = _jwtTokenService.GenerateTOken(user.Id, user.Username, new List<string> { user.Role });
        return token;
    }

}
