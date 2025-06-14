using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;

using User.Domain.CustomExceptions;
using User.Domain.Models;
using User.Application.Services.Interfaces;
using User.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace User.Application.Services;

public class LoginService : ILoginService
{
    protected IJwtTokenService _jwtTokenService;
    private readonly DataContext _context;
    private readonly PasswordHasher<User_> _hasher = new();

    public LoginService(IJwtTokenService jwtTokenService, DataContext context)
    {
        _jwtTokenService = jwtTokenService;
        _context = context;
       }

    public string Login(string username, string password)
    {
        var user = _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefault(u => u.Username == username);

        if (user == null)
            throw new InvalidCredentialsException();

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed)
            throw new InvalidCredentialsException();

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList(); 
        return _jwtTokenService.GenerateTOken(user.Id, user.Username, roles);
    }

}
