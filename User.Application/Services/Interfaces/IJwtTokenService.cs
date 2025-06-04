using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Application.Services.Interfaces;

public interface IJwtTokenService
{
    string GenerateTOken(int userId, string username,List<string> roles);
}
