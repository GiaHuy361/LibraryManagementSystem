using LMS.Application.DTOs.Users;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService) => _userService = userService;

    /// <summary>Get all users.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _userService.GetAllUsersAsync();
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }

    /// <summary>Get a specific user by ID.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _userService.GetUserByIdAsync(id);
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }

    /// <summary>UC-03: Admin creates a new user.</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        var result = await _userService.CreateUserAsync(dto);
        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { id = result.Data!.UserId }, result.Data)
            : StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }

    /// <summary>UC-04: Admin updates a user.</summary>
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserDto dto)
    {
        var result = await _userService.UpdateUserAsync(id, dto);
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }

    /// <summary>UC-05: Admin deletes a user.</summary>
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _userService.DeleteUserAsync(id);
        return result.IsSuccess ? NoContent() : StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }

    /// <summary>UC-06: Admin assigns a role to a user.</summary>
    [HttpPut("{id:int}/role")]
    public async Task<IActionResult> AssignRole(int id, [FromBody] AssignRoleDto dto)
    {
        var result = await _userService.AssignRoleAsync(id, dto);
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }
}
