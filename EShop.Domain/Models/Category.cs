using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Domain.Models;

public class Category : BaseModel
{
    public int id { get; set; }
    public string Name { get; set; } = default!;
    public Guid updated_by { get; set; }
    public DateTime updated_at { get; set; } = DateTime.UtcNow;
    public Guid created_by { get; set; }
    public DateTime created_at { get; set; } = DateTime.UtcNow;
    public bool deleted { get; set; } = false;
}
