using LMS.Application.Common;
using LMS.Application.DTOs.Books;
using LMS.Application.Services;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using Moq;
using FluentAssertions;
using Xunit;

namespace LMS.Application.Tests;

public class BookServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly BookService _bookService;

    public BookServiceTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _bookService = new BookService(_mockUow.Object);
    }

    [Fact]
    public async Task GetBookByIdAsync_ShouldReturnError_WhenNotFound()
    {
        // Arrange
        _mockUow.Setup(u => u.Books.GetWithDetailsAsync(1)).ReturnsAsync((Book?)null);

        // Act
        var result = await _bookService.GetBookByIdAsync(1);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetAllBooksAsync_ShouldReturnPagedResult()
    {
        // Arrange
        var books = new List<Book>
        {
            new() { BookId = 1, Title = "Book 1", Category = new Category { CategoryName = "Cat" }, BookAuthors = [] },
            new() { BookId = 2, Title = "Book 2", Category = new Category { CategoryName = "Cat" }, BookAuthors = [] }
        };
        _mockUow.Setup(u => u.Books.GetAllAsync()).ReturnsAsync(books);

        var pagination = new PaginationParams { PageNumber = 1, PageSize = 10 };

        // Act
        var result = await _bookService.GetAllBooksAsync(pagination);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Items.Count().Should().Be(2);
        result.Data!.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task CreateBookAsync_ShouldAddBookAndReturnSuccess()
    {
        // Arrange
        var dto = new CreateBookDto { Title = "New Book", CategoryId = 1, AuthorIds = [1] };
        _mockUow.Setup(u => u.Categories.GetByIdAsync(1)).ReturnsAsync(new Category());
        _mockUow.Setup(u => u.Authors.GetByIdAsync(1)).ReturnsAsync(new Author());
        _mockUow.Setup(u => u.Books.AddAsync(It.IsAny<Book>())).Returns(Task.CompletedTask);
        _mockUow.Setup(u => u.Books.GetWithDetailsAsync(It.IsAny<int>())).ReturnsAsync(new Book { BookId = 1, Title = "New Book" });

        // Act
        var result = await _bookService.CreateBookAsync(dto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Title.Should().Be("New Book");
        _mockUow.Verify(u => u.SaveChangesAsync(), Times.Exactly(2));
    }
}
