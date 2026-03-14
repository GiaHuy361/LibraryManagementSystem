using FluentAssertions;
using LMS.API.Controllers;
using LMS.Application.Common;
using LMS.Application.DTOs.Categories;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LMS.API.Tests.Controllers;

public class CategoriesControllerTests
{
    private readonly Mock<ICategoryService> _serviceMock;
    private readonly CategoriesController _controller;

    public CategoriesControllerTests()
    {
        _serviceMock = new Mock<ICategoryService>();
        _controller = new CategoriesController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithCategories()
    {
        // Arrange
        var categories = new List<CategoryDto> { new CategoryDto { CategoryId = 1, CategoryName = "Sci-Fi" } };
        _serviceMock.Setup(s => s.GetAllCategoriesAsync()).ReturnsAsync(Result<IEnumerable<CategoryDto>>.Success(categories));

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedData = Assert.IsAssignableFrom<IEnumerable<CategoryDto>>(okResult.Value);
        returnedData.Should().HaveCount(1);
    }

    [Fact]
    public async Task Create_ValidInput_ReturnsCreated()
    {
        // Arrange
        var dto = new CreateCategoryDto { CategoryName = "Science" };
        var createdDto = new CategoryDto { CategoryId = 2, CategoryName = "Science" };
        _serviceMock.Setup(s => s.CreateCategoryAsync(dto)).ReturnsAsync(Result<CategoryDto>.Success(createdDto));

        // Act
        var result = await _controller.Create(dto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedData = Assert.IsType<CategoryDto>(createdResult.Value);
        returnedData.CategoryName.Should().Be("Science");
    }

    [Fact]
    public async Task Delete_ExistingCategory_ReturnsNoContent()
    {
        // Arrange
        _serviceMock.Setup(s => s.DeleteCategoryAsync(1)).ReturnsAsync(Result.Success());

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}
