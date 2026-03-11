using LMS.Application.Common;
using LMS.Application.DTOs.Auth;

namespace LMS.Application.Interfaces;

/// <summary>UC-01 Login / UC-02 Logout</summary>
public interface IAuthService
{
    Task<Result<LoginResponseDto>> LoginAsync(LoginRequestDto dto);
}
