using FluentAssertions;
using LMS.API.Controllers;
using LMS.Application.Common;
using LMS.Application.DTOs.Authors;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LMS.API.Tests.Controllers;

public class AuthorsControllerTests
{
    private readonly Mock<IAuthorService> _serviceMock;
    private readonly AuthorsController _controller;

    public AuthorsControllerTests()
    {
        _serviceMock = new Mock<IAuthorService>();
        _controller = new AuthorsController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithAuthors()
    {
        // Arrange
        var authors = new List<AuthorDto> { new AuthorDto { AuthorId = 1, AuthorName = "J.R.R. Tolkien" } };
        _serviceMock.Setup(s => s.GetAllAuthorsAsync()).ReturnsAsync(Result<IEnumerable<AuthorDto>>.Success(authors));

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedData = Assert.IsAssignableFrom<IEnumerable<AuthorDto>>(okResult.Value);
        returnedData.Should().HaveCount(1);
    }

    [Fact]
    public async Task Create_ValidInput_ReturnsCreated()
    {
        // Arrange
        var dto = new CreateAuthorDto { AuthorName = "Isaac Asimov" };
        var createdDto = new AuthorDto { AuthorId = 2, AuthorName = "Isaac Asimov" };
        _serviceMock.Setup(s => s.CreateAuthorAsync(dto)).ReturnsAsync(Result<AuthorDto>.Success(createdDto));

        // Act
        var result = await _controller.Create(dto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedData = Assert.IsType<AuthorDto>(createdResult.Value);
        returnedData.AuthorName.Should().Be("Isaac Asimov");
    }
}
