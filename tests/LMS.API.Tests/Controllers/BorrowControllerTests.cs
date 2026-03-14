using FluentAssertions;
using LMS.API.Controllers;
using LMS.Application.Common;
using LMS.Application.DTOs.Borrows;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace LMS.API.Tests.Controllers;

public class BorrowControllerTests
{
    private readonly Mock<IBorrowService> _serviceMock;
    private readonly BorrowController _controller;

    public BorrowControllerTests()
    {
        _serviceMock = new Mock<IBorrowService>();
        _controller = new BorrowController(_serviceMock.Object);
        
        // Mock User Claims
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
        }));
        
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Fact]
    public async Task IssueBook_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var dto = new IssueBorrowDto { MemberId = 2, Books = new List<BorrowBookItemDto> { new BorrowBookItemDto { BookId = 5 } } };
        var createdDto = new BorrowRecordDto { BorrowId = 10, MemberName = "John" };
        _serviceMock.Setup(s => s.IssueBookAsync(1, dto)).ReturnsAsync(Result<BorrowRecordDto>.Success(createdDto));

        // Act
        var result = await _controller.IssueBook(dto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var returnedData = Assert.IsType<BorrowRecordDto>(createdResult.Value);
        returnedData.BorrowId.Should().Be(10);
    }

    [Fact]
    public async Task ReturnBook_ValidRequest_ReturnsOk()
    {
        // Arrange
        var returnedDto = new BorrowRecordDto { BorrowId = 10, TotalFine = 0 };
        _serviceMock.Setup(s => s.ReturnBookAsync(10, 1)).ReturnsAsync(Result<BorrowRecordDto>.Success(returnedDto));

        // Act
        var result = await _controller.ReturnBook(10);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedData = Assert.IsType<BorrowRecordDto>(okResult.Value);
        returnedData.TotalFine.Should().Be(0);
    }
}
