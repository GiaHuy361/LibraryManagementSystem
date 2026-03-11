using LMS.Application.Common;
using LMS.Application.DTOs.Users;
using LMS.Application.Interfaces;
using LMS.Domain.Entities;
using LMS.Domain.Interfaces;

namespace LMS.Application.Services;

/// <summary>
/// Handles UC-03 through UC-06: User CRUD and role assignment.
/// Only Admin can call these — enforced via [Authorize(Roles = "Admin")] in controller.
/// </summary>
public class UserService : IUserService
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(IUnitOfWork uow, IPasswordHasher passwordHasher)
    {
        _uow = uow;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<IEnumerable<UserDto>>> GetAllUsersAsync()
    {
        var users = await _uow.Users.GetAllAsync();
        return Result<IEnumerable<UserDto>>.Success(users.Select(MapToDto));
    }

    public async Task<Result<UserDto>> GetUserByIdAsync(int userId)
    {
        var user = await _uow.Users.GetWithRoleAsync(userId);
        if (user == null) return Result<UserDto>.NotFound($"User {userId} not found.");
        return Result<UserDto>.Success(MapToDto(user));
    }

    public async Task<Result<UserDto>> CreateUserAsync(CreateUserDto dto)
    {
        // Validate unique username
        var existing = await _uow.Users.GetByUsernameAsync(dto.Username);
        if (existing != null) return Result<UserDto>.Conflict($"Username '{dto.Username}' already exists.");

        // Validate role exists
        var role = await _uow.Roles.GetByIdAsync(dto.RoleId);
        if (role == null) return Result<UserDto>.BadRequest($"Role {dto.RoleId} does not exist.");

        // Factory approach: centralized entity creation
        var user = UserFactory.Create(dto, _passwordHasher);
        await _uow.Users.AddAsync(user);
        await _uow.SaveChangesAsync();

        user.Role = role;
        return Result<UserDto>.Created(MapToDto(user));
    }

    public async Task<Result<UserDto>> UpdateUserAsync(int userId, UpdateUserDto dto)
    {
        var user = await _uow.Users.GetByIdAsync(userId);
        if (user == null) return Result<UserDto>.NotFound($"User {userId} not found.");

        if (dto.FullName != null) user.FullName = dto.FullName;
        if (dto.Email != null) user.Email = dto.Email;
        if (dto.Phone != null) user.Phone = dto.Phone;
        if (dto.Status != null) user.Status = dto.Status;

        _uow.Users.Update(user);
        await _uow.SaveChangesAsync();
        return Result<UserDto>.Success(MapToDto(user));
    }

    public async Task<Result> DeleteUserAsync(int userId)
    {
        var user = await _uow.Users.GetByIdAsync(userId);
        if (user == null) return Result.NotFound($"User {userId} not found.");

        _uow.Users.Remove(user);
        await _uow.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result<UserDto>> AssignRoleAsync(int userId, AssignRoleDto dto)
    {
        var user = await _uow.Users.GetWithRoleAsync(userId);
        if (user == null) return Result<UserDto>.NotFound($"User {userId} not found.");

        var role = await _uow.Roles.GetByIdAsync(dto.RoleId);
        if (role == null) return Result<UserDto>.BadRequest($"Role {dto.RoleId} does not exist.");

        user.RoleId = dto.RoleId;
        user.Role = role;
        _uow.Users.Update(user);
        await _uow.SaveChangesAsync();
        return Result<UserDto>.Success(MapToDto(user));
    }

    private static UserDto MapToDto(User u) => new()
    {
        UserId = u.UserId,
        Username = u.Username,
        FullName = u.FullName,
        Email = u.Email,
        Phone = u.Phone,
        RoleId = u.RoleId,
        RoleName = u.Role?.RoleName,
        Status = u.Status,
        CreatedAt = u.CreatedAt
    };
}

/// <summary>
/// Factory Pattern: centralizes User entity creation logic.
/// </summary>
public static class UserFactory
{
    public static User Create(CreateUserDto dto, IPasswordHasher passwordHasher) => new()
    {
        Username = dto.Username,
        PasswordHash = passwordHasher.Hash(dto.Password),
        FullName = dto.FullName,
        Email = dto.Email,
        Phone = dto.Phone,
        RoleId = dto.RoleId,
        Status = "Active",
        CreatedAt = DateTime.UtcNow
    };
}
