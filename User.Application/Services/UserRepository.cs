using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Domain.Models;

namespace User.Application.Services;

public class UserRepository : IUserRepository
{
    private readonly List<UserAccount> _users = new()
    {
        new UserAccount { Id = 1, Username = "admin", Password = "password123", Role = "Administrator" },
        new UserAccount { Id = 2, Username = "Ola", Password = "ufohom123", Role = "Employee" },
        new UserAccount { Id = 3, Username = "Weronika", Password = "balsam12!", Role = "Employee" },
    };

    public UserAccount? GetByUsername(string username) =>
        _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

    public void Add(UserAccount user)
    {
        user.Id = _users.Max(u => u.Id) + 1;
        _users.Add(user);
    }

    public IEnumerable<UserAccount> GetAll() => _users;
}