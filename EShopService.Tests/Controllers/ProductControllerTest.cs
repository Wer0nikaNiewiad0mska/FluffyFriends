using Moq;
using Xunit;
using EShop.Application.Interfaces;
using EShopService.Controllers;
using Microsoft.AspNetCore.Mvc;
using EShop.Domain.Models;

namespace EShopService.Tests.Controllers;

public class ProductControllerTest
{
    private readonly Mock<IProductService> _mockService;
    private readonly ProductController _controller;

    public ProductControllerTest()
    {
        _mockService = new Mock<IProductService>();
        _controller = new ProductController(_mockService.Object);
    }

    [Fact]
    public async Task Get_ShouldReturnAllProducts()
    {
        var products = new List<Product> { new Product(), new Product() };
        _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(products);

        var result = await _controller.Get();
        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        Assert.Equal(products, okResult.Value);
    }

    [Fact]
    public async Task Get_WithValidId_ReturnsProduct()
    {
        var product = new Product { id = 1 };
        _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(product);

        var result = await _controller.Get(1);
        var okResult = Assert.IsType<OkObjectResult>(result.Result);

        Assert.Equal(product, okResult.Value);
    }

    [Fact]
    public async Task Get_WithInvalidId_ReturnsNotFound()
    {
        _mockService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product?)null);

        var result = await _controller.Get(999);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task Post_ValidProduct_ReturnsCreatedProduct()
    {
        var newProduct = new Product();

        var result = await _controller.Post(newProduct);

        _mockService.Verify(s => s.AddAsync(newProduct), Times.Once);

        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(newProduct, createdAtActionResult.Value);
    }

    [Fact]
    public async Task Put_ValidProduct_UpdatesAndReturnsNoContent()
    {
        var product = new Product { id = 1 };
        _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(product);

        var result = await _controller.Put(1, product);

        _mockService.Verify(s => s.UpdateAsync(product), Times.Once);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Put_InvalidProduct_ReturnsNotFound()
    {
        _mockService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product?)null);

        var result = await _controller.Put(999, new Product());

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ValidId_ReturnsNoContent()
    {
        var product = new Product { id = 1, deleted = false };
        _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(product);

        var result = await _controller.Delete(1);

        _mockService.Verify(s => s.DeleteAsync(1), Times.Once);
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Delete_InvalidId_ReturnsNotFound()
    {
        _mockService.Setup(s => s.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Product?)null);

        var result = await _controller.Delete(999);

        Assert.IsType<NotFoundResult>(result);
    }
}
