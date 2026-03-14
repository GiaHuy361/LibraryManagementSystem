using FluentAssertions;
using LMS.API.Controllers;
using LMS.Application.DTOs.Users;
using LMS.Application.Interfaces;
using LMS.Application.Common;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace LMS.API.Tests.Controllers;

public class UsersControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly UsersController _controller;

    public UsersControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _controller = new UsersController(_userServiceMock.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithUsers()
    {
        // Arrange
        var users = new List<UserDto> { new UserDto { UserId = 1, Username = "testuser" } };
        _userServiceMock.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(Result<IEnumerable<UserDto>>.Success(users));

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedUsers = Assert.IsAssignableFrom<IEnumerable<UserDto>>(okResult.Value);
        returnedUsers.Should().HaveCount(1);
    }

    [Fact]
    public async Task Create_ValidUser_ReturnsCreatedAtAction()
    {
        // Arrange
        var request = new CreateUserDto { Username = "newuser" };
        var createdUser = new UserDto { UserId = 2, Username = "newuser" };
        _userServiceMock.Setup(s => s.CreateUserAsync(request)).ReturnsAsync(Result<UserDto>.Success(createdUser));

        // Act
        var result = await _controller.Create(request);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        createdAtActionResult.ActionName.Should().Be(nameof(UsersController.GetById));
        createdAtActionResult.RouteValues["id"].Should().Be(2);
        var returnedUser = Assert.IsType<UserDto>(createdAtActionResult.Value);
        returnedUser.Username.Should().Be("newuser");
    }

    [Fact]
    public async Task Delete_ExistingUser_ReturnsNoContent()
    {
        // Arrange
        _userServiceMock.Setup(s => s.DeleteUserAsync(1)).ReturnsAsync(Result.Success());

        // Act
        var result = await _controller.Delete(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
}
