using LMS.Application.DTOs.Users;
using LMS.Application.Interfaces;
using LMS.Application.Services;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;
using Moq;
using FluentAssertions;
using Xunit;

namespace LMS.Application.Tests;

public class UserServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUow;
    private readonly Mock<IPasswordHasher> _mockHasher;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUow = new Mock<IUnitOfWork>();
        _mockHasher = new Mock<IPasswordHasher>();
        _userService = new UserService(_mockUow.Object, _mockHasher.Object);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldReturnConflict_WhenUsernameExists()
    {
        // Arrange
        var dto = new CreateUserDto { Username = "testuser", Email = "a@b.com", Password = "123", FullName = "Test", RoleId = 2 };
        _mockUow.Setup(u => u.Users.GetByUsernameAsync("testuser")).ReturnsAsync(new User());

        // Act
        var result = await _userService.CreateUserAsync(dto);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.StatusCode.Should().Be(409);
        result.ErrorMessage.Should().Contain("already exists");
    }

    [Fact]
    public async Task CreateUserAsync_ShouldHashPasswordAndCreateUser_WhenValid()
    {
        // Arrange
        var dto = new CreateUserDto { Username = "newuser", Email = "a@b.com", Password = "rawpassword", FullName = "Test", RoleId = 2 };
        _mockUow.Setup(u => u.Users.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        _mockUow.Setup(u => u.Users.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        _mockUow.Setup(u => u.Roles.GetByIdAsync(2)).ReturnsAsync(new Role { RoleId = 2, RoleName = "Librarian" });
        _mockHasher.Setup(h => h.Hash("rawpassword")).Returns("hashed_password");
        _mockUow.Setup(u => u.Users.AddAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

        // Act
        var result = await _userService.CreateUserAsync(dto);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Data!.Username.Should().Be("newuser");
        _mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);
    }
}
