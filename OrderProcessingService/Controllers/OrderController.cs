using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderProcessingService.Models;
using OrderProcessingService.Services;
using System.Security.Claims;

namespace OrderProcessingService.Controllers;

[Authorize]
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
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized("Nieprawidłowy token lub brak ID użytkownika");

        var orderId = await _processor.ProcessOrderAsync(userId, request);
        return Ok(new { orderId });
    }
}
