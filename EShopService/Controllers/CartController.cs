﻿using System.Security.Claims;
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

        if (cart == null)
            return Ok(new CartDto()); // pusty koszyk

        var dto = new CartDto
        {
            Items = cart.Items.Select(i => new CartItemDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product.Name,
                Quantity = i.Quantity,
                UnitPrice = i.Product.price
            }).ToList()
        };

        return Ok(dto);
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

    private int GetUserId()
    {
        // ClaimTypes.NameIdentifier = "nameid" to domyślny klucz w tokenie
        var claim = User.FindFirst(ClaimTypes.NameIdentifier);

        if (claim == null || !int.TryParse(claim.Value, out var id))
            throw new UnauthorizedAccessException("Brak ID użytkownika w tokenie");

        return id;
    }
}