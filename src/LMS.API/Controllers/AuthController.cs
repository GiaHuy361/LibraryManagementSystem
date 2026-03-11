using LMS.Application.DTOs.Auth;
using LMS.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService) => _authService = authService;

    /// <summary>UC-01: Login with username and password, returns JWT token.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
    {
        var result = await _authService.LoginAsync(dto);
        return result.IsSuccess ? Ok(result.Data) : StatusCode(result.StatusCode, new { message = result.ErrorMessage });
    }

    /// <summary>UC-02: Logout (client-side token discard — server is stateless).</summary>
    [HttpPost("logout")]
    [Authorize]
    public IActionResult Logout() => Ok(new { message = "Logged out successfully. Please discard your token." });
}
