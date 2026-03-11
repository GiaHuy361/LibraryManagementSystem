using LMS.Application.DTOs.Authors;
using LMS.Application.Services;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using Moq;
using FluentAssertions;
using Xunit;

namespace LMS.Application.Tests;

public class AuthorServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly AuthorService _authorService;

    public AuthorServiceTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _authorService = new AuthorService(_mockUow.Object);
    }

    [Fact]
    public async Task GetAllAuthorsAsync_ShouldReturnAllAuthors()
    {
        // Arrange
        var authors = new List<Author>
        {
            new() { AuthorId = 1, AuthorName = "Author1" },
            new() { AuthorId = 2, AuthorName = "Author2" }
        };
        _mockUow.Setup(u => u.Authors.GetAllAsync()).ReturnsAsync(authors);

        // Act
        var result = await _authorService.GetAllAuthorsAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Count().Should().Be(2);
    }

    [Fact]
    public async Task GetAuthorByIdAsync_ShouldReturnError_WhenMissing()
    {
        // Arrange
        _mockUow.Setup(u => u.Authors.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Author?)null);

        // Act
        var result = await _authorService.GetAuthorByIdAsync(999);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task CreateAuthorAsync_ShouldAddAuthor()
    {
        // Arrange
        var dto = new CreateAuthorDto { AuthorName = "New Author", Biography = "Bio" };
        _mockUow.Setup(u => u.Authors.AddAsync(It.IsAny<Author>())).Returns(Task.CompletedTask);

        // Act
        var result = await _authorService.CreateAuthorAsync(dto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.AuthorName.Should().Be("New Author");
        _mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
