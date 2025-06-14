using Microsoft.AspNetCore.Mvc;

namespace ReceiptService.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("ReceiptService is running");
}
