using FluentAssertions;
using LMS.API.Controllers;
using LMS.Application.Common;
using LMS.Application.DTOs.Books;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LMS.API.Tests.Controllers;

public class BooksControllerTests
{
    private readonly Mock<IBookService> _bookServiceMock;
    private readonly BooksController _controller;

    public BooksControllerTests()
    {
        _bookServiceMock = new Mock<IBookService>();
        _controller = new BooksController(_bookServiceMock.Object);
    }

    [Fact]
    public async Task GetById_ExistingBook_ReturnsOkWithBook()
    {
        // Arrange
        var book = new BookDto { BookId = 1, Title = "Clean Code" };
        _bookServiceMock.Setup(s => s.GetBookByIdAsync(1)).ReturnsAsync(Result<BookDto>.Success(book));

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedBook = Assert.IsType<BookDto>(okResult.Value);
        returnedBook.Title.Should().Be("Clean Code");
    }

    [Fact]
    public async Task GetById_NonExistingBook_ReturnsNotFound()
    {
        // Arrange
        _bookServiceMock.Setup(s => s.GetBookByIdAsync(99)).ReturnsAsync(Result<BookDto>.Failure("Book not found", 404));

        // Act
        var result = await _controller.GetById(99);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        statusCodeResult.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Search_ValidQuery_ReturnsOkWithResults()
    {
        // Arrange
        var searchDto = new BookSearchDto { Title = "Clean" };
        var books = new List<BookDto> { new BookDto { Title = "Clean Code" } };
        var pagedResult = new PagedResult<BookDto>(books, 1, 1, 10);
        _bookServiceMock.Setup(s => s.SearchBooksAsync(searchDto)).ReturnsAsync(Result<PagedResult<BookDto>>.Success(pagedResult));

        // Act
        var result = await _controller.Search(searchDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedBooks = Assert.IsType<PagedResult<BookDto>>(okResult.Value);
        returnedBooks.Items.Should().HaveCount(1);
    }
}
