using Microsoft.AspNetCore.Mvc;
using OrderProcessingService.Models;
using OrderProcessingService.Services;

namespace OrderProcessingService.Controllers;

[ApiController]
[Route("api/order")]
public class OrderController : ControllerBase
{
    private readonly OrderProcessor _processor;

    public OrderController(OrderProcessor processor)
    {
        _processor = processor;
    }

    [HttpPost("process")]
    public async Task<IActionResult> ProcessOrder([FromBody] ProcessOrderRequest request)
    {
        var orderId = await _processor.ProcessOrderAsync(request);
        return Ok(new { orderId });
    }
}
