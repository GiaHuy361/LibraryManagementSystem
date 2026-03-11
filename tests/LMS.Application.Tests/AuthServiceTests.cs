using LMS.Application.DTOs.Auth;
using LMS.Application.Interfaces;
using LMS.Application.Services;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using Moq;
using FluentAssertions;
using Xunit;

namespace LMS.Application.Tests;

public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IJwtTokenService> _mockJwt;
    private readonly Mock<IPasswordHasher> _mockHasher;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockJwt = new Mock<IJwtTokenService>();
        _mockHasher = new Mock<IPasswordHasher>();
        _authService = new AuthService(_mockUow.Object, _mockJwt.Object, _mockHasher.Object);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnError_WhenUserNotFound()
    {
        // Arrange
        var request = new LoginRequestDto { Username = "wrong", Password = "123" };
        _mockUow.Setup(u => u.Users.GetByUsernameAsync("wrong")).ReturnsAsync((User?)null);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnError_WhenPasswordIncorrect()
    {
        // Arrange
        var request = new LoginRequestDto { Username = "admin", Password = "wrongpassword" };
        var user = new User { Username = "admin", PasswordHash = "hashed" };
        _mockUow.Setup(u => u.Users.GetByUsernameAsync("admin")).ReturnsAsync(user);
        _mockHasher.Setup(h => h.Verify("wrongpassword", "hashed")).Returns(false);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsValid()
    {
        // Arrange
        var request = new LoginRequestDto { Username = "admin", Password = "correctpassword" };
        var user = new User { UserId = 1, Username = "admin", PasswordHash = "hashed" };
        _mockUow.Setup(u => u.Users.GetByUsernameAsync("admin")).ReturnsAsync(user);
        _mockHasher.Setup(h => h.Verify("correctpassword", "hashed")).Returns(true);
        _mockJwt.Setup(j => j.GenerateToken(user.UserId, user.Username, "Member")).Returns("fake-jwt-token");

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Token.Should().Be("fake-jwt-token");
    }
}
