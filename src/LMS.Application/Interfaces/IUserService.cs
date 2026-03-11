using LMS.Application.Common;
using LMS.Application.DTOs.Users;

namespace LMS.Application.Interfaces;

/// <summary>UC-03 Create User / UC-04 Update User / UC-05 Delete User / UC-06 Assign Role</summary>
public interface IUserService
{
    Task<Result<IEnumerable<UserDto>>> GetAllUsersAsync();
    Task<Result<UserDto>> GetUserByIdAsync(int userId);
    Task<Result<UserDto>> CreateUserAsync(CreateUserDto dto);
    Task<Result<UserDto>> UpdateUserAsync(int userId, UpdateUserDto dto);
    Task<Result> DeleteUserAsync(int userId);
    Task<Result<UserDto>> AssignRoleAsync(int userId, AssignRoleDto dto);
}
