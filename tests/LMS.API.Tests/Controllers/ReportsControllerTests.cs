using FluentAssertions;
using LMS.API.Controllers;
using LMS.Application.Common;
using LMS.Application.DTOs.Fines;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LMS.API.Tests.Controllers;

public class ReportsControllerTests
{
    private readonly Mock<IReportService> _serviceMock;
    private readonly ReportsController _controller;

    public ReportsControllerTests()
    {
        _serviceMock = new Mock<IReportService>();
        _controller = new ReportsController(_serviceMock.Object);
    }

    [Fact]
    public async Task GetSummary_ReturnsOkWithReport()
    {
        // Arrange
        var report = new ReportDto { TotalBooks = 100, TotalBorrows = 50 };
        _serviceMock.Setup(s => s.GetSummaryReportAsync()).ReturnsAsync(Result<ReportDto>.Success(report));

        // Act
        var result = await _controller.GetSummary();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedData = Assert.IsType<ReportDto>(okResult.Value);
        returnedData.TotalBooks.Should().Be(100);
        returnedData.TotalBorrows.Should().Be(50);
    }
}
