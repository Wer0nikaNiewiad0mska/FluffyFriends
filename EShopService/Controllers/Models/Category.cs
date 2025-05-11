namespace EShopService.Controllers.Models;

public class Category : BaseModel
{
    public int id { get; set; }
    public string name { get; set; } = default!;
    public Guid updated_by { get; set; }
    public DateTime updated_at { get; set; } = DateTime.UtcNow;
    public Guid created_by { get; set; }
    public DateTime created_at { get; set; } = DateTime.UtcNow;
    public Boolean deleted { get; set; } = false;
}
