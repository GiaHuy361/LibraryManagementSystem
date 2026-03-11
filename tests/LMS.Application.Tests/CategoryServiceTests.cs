using LMS.Application.DTOs.Categories;
using LMS.Application.Services;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using Moq;
using FluentAssertions;
using Xunit;

namespace LMS.Application.Tests;

public class CategoryServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly CategoryService _categoryService;

    public CategoryServiceTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _categoryService = new CategoryService(_mockUow.Object);
    }

    [Fact]
    public async Task GetAllCategoriesAsync_ShouldReturnAllCategories()
    {
        // Arrange
        var categories = new List<Category>
        {
            new() { CategoryId = 1, CategoryName = "Cat1" },
            new() { CategoryId = 2, CategoryName = "Cat2" }
        };
        _mockUow.Setup(u => u.Categories.GetAllAsync()).ReturnsAsync(categories);

        // Act
        var result = await _categoryService.GetAllCategoriesAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Count().Should().Be(2);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_ShouldReturnCategory_WhenGivenValidId()
    {
        // Arrange
        var cat = new Category { CategoryId = 1, CategoryName = "Cat1" };
        _mockUow.Setup(u => u.Categories.GetByIdAsync(1)).ReturnsAsync(cat);

        // Act
        var result = await _categoryService.GetCategoryByIdAsync(1);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.CategoryName.Should().Be("Cat1");
    }

    [Fact]
    public async Task GetCategoryByIdAsync_ShouldReturnError_WhenCategoryNotFound()
    {
        // Arrange
        _mockUow.Setup(u => u.Categories.GetByIdAsync(999)).ReturnsAsync((Category?)null);

        // Act
        var result = await _categoryService.GetCategoryByIdAsync(999);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
        result.ErrorMessage.Should().Contain("Category 999 not found.");
    }

    [Fact]
    public async Task CreateCategoryAsync_ShouldAddCategoryAndReturnSuccess()
    {
        // Arrange
        var dto = new CreateCategoryDto { CategoryName = "New Cat", Description = "Desc" };
        _mockUow.Setup(u => u.Categories.AddAsync(It.IsAny<Category>())).Returns(Task.CompletedTask);

        // Act
        var result = await _categoryService.CreateCategoryAsync(dto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.CategoryName.Should().Be("New Cat");
        _mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateCategoryAsync_ShouldUpdateAndReturnSuccess()
    {
        // Arrange
        var existingCat = new Category { CategoryId = 1, CategoryName = "Old", Description = "OldDesc" };
        _mockUow.Setup(u => u.Categories.GetByIdAsync(1)).ReturnsAsync(existingCat);

        var dto = new UpdateCategoryDto { CategoryName = "New", Description = "NewDesc" };

        // Act
        var result = await _categoryService.UpdateCategoryAsync(1, dto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        existingCat.CategoryName.Should().Be("New");
        existingCat.Description.Should().Be("NewDesc");
        _mockUow.Verify(u => u.Categories.Update(existingCat), Times.Once);
        _mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task DeleteCategoryAsync_ShouldDeleteAndReturnSuccess()
    {
        // Arrange
        var existingCat = new Category { CategoryId = 1, CategoryName = "Old" };
        _mockUow.Setup(u => u.Categories.GetByIdAsync(1)).ReturnsAsync(existingCat);

        // Act
        var result = await _categoryService.DeleteCategoryAsync(1);

        // Assert
        result.IsSuccess.Should().BeTrue();
        _mockUow.Verify(u => u.Categories.Remove(existingCat), Times.Once);
        _mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
