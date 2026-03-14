using FluentAssertions;
using LMS.API.Controllers;
using LMS.Application.Common;
using LMS.Application.DTOs.Auth;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LMS.API.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _controller = new AuthController(_authServiceMock.Object);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsOkWithToken()
    {
        // Arrange
        var request = new LoginRequestDto { Username = "admin", Password = "password123" };
        var responseDto = new LoginResponseDto { Token = "mock_jwt_token", Username = "admin" };
        
        _authServiceMock
            .Setup(s => s.LoginAsync(It.IsAny<LoginRequestDto>()))
            .ReturnsAsync(Result<LoginResponseDto>.Success(responseDto));

        // Act
        var result = await _controller.Login(request);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedDto = Assert.IsType<LoginResponseDto>(okResult.Value);
        returnedDto.Token.Should().Be("mock_jwt_token");
        returnedDto.Username.Should().Be("admin");
    }

    [Fact]
    public async Task Login_InvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var request = new LoginRequestDto { Username = "admin", Password = "wrongpassword" };
        
        _authServiceMock
            .Setup(s => s.LoginAsync(It.IsAny<LoginRequestDto>()))
            .ReturnsAsync(Result<LoginResponseDto>.Failure("Invalid username or password.", 401));

        // Act
        var result = await _controller.Login(request);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        statusCodeResult.StatusCode.Should().Be(401);
    }

    [Fact]
    public void Logout_ReturnsOkMessage()
    {
        // Act
        var result = _controller.Logout();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        okResult.Value.Should().NotBeNull();
    }
}
