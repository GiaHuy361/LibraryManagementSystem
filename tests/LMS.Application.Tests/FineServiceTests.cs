using LMS.Application.Services;
using LMS.Application.Strategies;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using Moq;
using FluentAssertions;
using Xunit;

namespace LMS.Application.Tests;

public class FineServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly FineService _fineService;

    public FineServiceTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _fineService = new FineService(_mockUow.Object);
    }

    [Fact]
    public async Task GetUnpaidFinesAsync_ShouldReturnFines()
    {
        // Arrange
        var fines = new List<Fine>
        {
            new() { FineId = 1, BorrowDetailId = 1, FineAmount = 5000, PaidStatus = false },
            new() { FineId = 2, BorrowDetailId = 2, FineAmount = 10000, PaidStatus = false }
        };
        _mockUow.Setup(u => u.Fines.GetUnpaidFinesAsync()).ReturnsAsync(fines);

        // Act
        var result = await _fineService.GetUnpaidFinesAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Count().Should().Be(2);
        result.Data!.Sum(f => f.FineAmount).Should().Be(15000);
    }

    [Fact]
    public async Task PayFineAsync_ShouldReturnError_WhenFineNotFound()
    {
        // Arrange
        _mockUow.Setup(u => u.Fines.GetByIdAsync(999)).ReturnsAsync((Fine?)null);

        // Act
        var result = await _fineService.PayFineAsync(999);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task PayFineAsync_ShouldReturnError_WhenAlreadyPaid()
    {
        // Arrange
        var fine = new Fine { FineId = 1, PaidStatus = true };
        _mockUow.Setup(u => u.Fines.GetByIdAsync(1)).ReturnsAsync(fine);

        // Act
        var result = await _fineService.PayFineAsync(1);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(400);
        result.ErrorMessage.Should().Contain("already been paid");
    }

    [Fact]
    public async Task PayFineAsync_ShouldUpdateStatusToPaid()
    {
        // Arrange
        var fine = new Fine { FineId = 1, PaidStatus = false };
        _mockUow.Setup(u => u.Fines.GetByIdAsync(1)).ReturnsAsync(fine);

        // Act
        var result = await _fineService.PayFineAsync(1);

        // Assert
        result.IsSuccess.Should().BeTrue();
        fine.PaidStatus.Should().Be(true);
        _mockUow.Verify(u => u.Fines.Update(fine), Times.Once);
        _mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
