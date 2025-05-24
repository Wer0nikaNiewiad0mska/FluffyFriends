using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using User.Domain.Models;

namespace User.Application.Services;

public interface IUserRepository
{
    UserAccount? GetByUsername(string username);
    void Add(UserAccount user);
    IEnumerable<UserAccount> GetAll();
}
