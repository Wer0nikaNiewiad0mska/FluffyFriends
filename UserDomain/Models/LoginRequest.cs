using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Domain.Models;
public class LoginRequest
{
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
}
