using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Application.Services.Interfaces;

public interface ILoginService
{
    string Login(string username, string password);
}
