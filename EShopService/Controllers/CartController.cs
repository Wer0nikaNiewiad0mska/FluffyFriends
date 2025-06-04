using System.Security.Claims;
using EShop.Application.Interfaces;
using EShop.Domain.Models;
using EShop.Domain.Repositories;
using EShopService.Controllers.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShopService.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController : ControllerBase
{
    private readonly ICartRepository _cartRepository;

    public CartController(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart()
    {
        var userId = GetUserId();
        var cart = await _cartRepository.GetByUserIdAsync(userId);
        return Ok(cart?.Items ?? []);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItem([FromBody] AddCartItemDto dto)
    {
        var userId = GetUserId();
        await _cartRepository.AddOrUpdateItemAsync(userId, dto.ProductId, dto.Quantity);
        return NoContent();
    }

    [HttpDelete("items/{productId}")]
    public async Task<IActionResult> RemoveItem(int productId)
    {
        var userId = GetUserId();
        await _cartRepository.RemoveItemAsync(userId, productId);
        return NoContent();
    }

    [HttpDelete]
    public async Task<IActionResult> ClearCart()
    {
        var userId = GetUserId();
        await _cartRepository.ClearCartAsync(userId);
        return NoContent();
    }

    private Guid GetUserId()
    {
        var claim = User.FindFirst("userId");
        return claim != null ? Guid.Parse(claim.Value) : throw new UnauthorizedAccessException();
    }
}
