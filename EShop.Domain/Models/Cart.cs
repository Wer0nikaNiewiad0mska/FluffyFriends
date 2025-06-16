using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EShop.Domain.Models;

public class Cart : BaseModel
{
    public int id { get; set; }
    public int UserId { get; set; }
    public ICollection<CartItem> Items { get; set; } = new List<CartItem>();
}
