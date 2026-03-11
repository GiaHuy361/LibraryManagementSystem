using LMS.Application.Common;
using LMS.Application.DTOs.Auth;
using LMS.Application.Interfaces;
using LMS.Domain.Interfaces;

namespace LMS.Application.Services;

/// <summary>
/// Handles UC-01 Login using username/password validation and JWT generation.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUnitOfWork _uow;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(IUnitOfWork uow, IJwtTokenService jwtTokenService, IPasswordHasher passwordHasher)
    {
        _uow = uow;
        _jwtTokenService = jwtTokenService;
        _passwordHasher = passwordHasher;
    }

    public async Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto dto)
    {
        var user = await _uow.Users.GetByUsernameAsync(dto.Username);

        if (user == null)
            return Result<LoginResponseDto>.Unauthorized("Invalid username or password.");

        // Verify password hash
        if (!_passwordHasher.Verify(dto.Password, user.PasswordHash))
            return Result<LoginResponseDto>.Unauthorized("Invalid username or password.");

        if (!user.IsActive())
            return Result<LoginResponseDto>.Unauthorized("Your account has been deactivated.");

        // Load role for token
        var userWithRole = await _uow.Users.GetWithRoleAsync(user.UserId);
        var roleName = userWithRole?.Role?.RoleName ?? "Member";

        var expiresAt = DateTime.UtcNow.AddHours(8);
        var token = _jwtTokenService.GenerateToken(user.UserId, user.Username, roleName);

        return Result<LoginResponseDto>.Success(new LoginResponseDto
        {
            Token = token,
            ExpiresAt = expiresAt,
            UserId = user.UserId,
            Username = user.Username,
            FullName = user.FullName ?? user.Username,
            Role = roleName
        });
    }
}
