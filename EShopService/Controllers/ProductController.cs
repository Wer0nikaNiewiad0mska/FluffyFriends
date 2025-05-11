using EShop.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using EShop.Domain.Models;

namespace EShopService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Product>>> Get()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> Get(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
            return NotFound();
        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Product product)
    {
        await _productService.AddAsync(product);
        return CreatedAtAction(nameof(Get), new { id = product.id }, product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] Product updatedProduct)
    {
        var existing = await _productService.GetByIdAsync(id);
        if (existing == null)
            return NotFound();

        updatedProduct.id = id;
        await _productService.UpdateAsync(updatedProduct);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await _productService.GetByIdAsync(id);
        if (product == null)
            return NotFound();

        await _productService.DeleteAsync(id);
        return NoContent();
    }
}