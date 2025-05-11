using System.ComponentModel;

namespace EShopService.Controllers.Models;

public class Product : BaseModel
{
    public int id { get; set; }
    public string name { get; set; } = default!;
    public string ean { get; set; }
    public decimal price { get; set; }
    public int stock { get; set; }
    public string sku { get; set; }

    public Category category { get; set; }
    public Guid updated_by { get; set; }
    public DateTime updated_at { get; set; } = DateTime.UtcNow;
    public Guid created_by { get; set; }
    public DateTime created_at { get; set; } = DateTime.UtcNow;
    public Boolean deleted { get; set; } = false;
}
