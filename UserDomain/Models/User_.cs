using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace User.Domain.Models;

public class User_
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Username { get; set; } = default!;

    [Required]
    public string Email { get; set; } = default!;

    [Required]
    public string PasswordHash { get; set; } = default!;

    public ICollection<Role> Roles { get; set; } = new List<Role>();

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastLoginAt { get; set; }

    public bool IsActive { get; set; } = true;
}