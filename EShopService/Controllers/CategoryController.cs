using EShop.Application.Interfaces;
using EShop.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace EShopService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _categoryService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var category = await _categoryService.GetByIdAsync(id);
        return category is null ? NotFound() : Ok(category);
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Category category)
    {
        await _categoryService.AddAsync(category);
        return CreatedAtAction(nameof(Get), new { id = category.id }, category);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Put(int id, [FromBody] Category category)
    {
        category.id = id;
        await _categoryService.UpdateAsync(category);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _categoryService.DeleteAsync(id);
        return NoContent();
    }
}
