using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Domain.Models;

public class UserRole
{
    public int UserId { get; set; }
    public User_ User { get; set; } = default!;

    public int RoleId { get; set; }
    public Role Role { get; set; } = default!;
}