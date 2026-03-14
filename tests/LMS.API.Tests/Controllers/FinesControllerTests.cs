using FluentAssertions;
using LMS.API.Controllers;
using LMS.Application.Common;
using LMS.Application.DTOs.Fines;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LMS.API.Tests.Controllers;

public class FinesControllerTests
{
    private readonly Mock<IFineService> _serviceMock;
    private readonly FinesController _controller;

    public FinesControllerTests()
    {
        _serviceMock = new Mock<IFineService>();
        _controller = new FinesController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetByBorrowDetail_ReturnsOkWithFine()
    {
        // Arrange
        var fine = new FineDto { FineId = 1, FineAmount = 10, PaidStatus = false };
        _serviceMock.Setup(s => s.GetFineByBorrowDetailAsync(1)).ReturnsAsync(Result<FineDto>.Success(fine));

        // Act
        var result = await _controller.GetByBorrowDetail(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedData = Assert.IsType<FineDto>(okResult.Value);
        returnedData.FineAmount.Should().Be(10);
    }

    [Fact]
    public async Task Pay_ValidFineId_ReturnsOk()
    {
        // Arrange
        _serviceMock.Setup(s => s.PayFineAsync(1)).ReturnsAsync(Result<FineDto>.Success(new FineDto { FineId = 1 }));

        // Act
        var result = await _controller.Pay(1);

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }
}
