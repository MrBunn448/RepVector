using Microsoft.AspNetCore.Mvc;
using WorkoutTracker.Logic.Services;
using WorkoutTracker.Api.Infrastructure;

namespace WorkoutTracker.Api.Controllers;

/// Endpoints for user registration, login, and administrative account creation.
[ApiController]
[Route("api/[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var result = await authService.RegisterAsync(request.Email, request.Password);
        
        if (result.IsSuccess)
        {
            var user = result.Value;
            return Ok(new { userId = user?.Id, email = user?.Email, message = "Registration successful." });
        }

        return result.ToActionResult();
    }

    /// <returns>An OK response with user details and role if successful.
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await authService.LoginAsync(request.Email, request.Password);

        if (result.IsSuccess)
        {
            var user = result.Value;
            return Ok(new { userId = user?.Id, email = user?.Email, role = user?.Role, message = "Login successful." });
        }

        return result.ToActionResult(); // automatically converts C# result into the correct HTTP response.
    }
    /// Requires a system-configured admin secret.
    [HttpPost("register/admin")]
    public async Task<IActionResult> RegisterAdmin([FromBody] AdminRegisterRequest request)
    {
        var result = await authService.RegisterAdminAsync(request.Email, request.Password, request.AdminSecret);

        if (result.IsSuccess)
        {
            var user = result.Value;
            return Ok(new { userId = user?.Id, email = user?.Email, role = user?.Role, message = "Admin registration successful." });
        }

        return result.ToActionResult();
    }
}


/// DTO
public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

/// DTO
public class RegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AdminRegisterRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string AdminSecret { get; set; } = string.Empty;
}
