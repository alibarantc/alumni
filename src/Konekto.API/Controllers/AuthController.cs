using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Konekto.API.Models;
using Konekto.API.Services;
using Konekto.Business.Security;
using Konekto.Domain.Entities;
using Konekto.Domain.Interfaces;

namespace Konekto.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly JwtTokenService _jwtTokenService;

    public AuthController(IUserRepository userRepository, JwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
    }

    /// <summary>
    /// Whiteboard Client-Server Token Authentication:
    /// Client sends Username/Password -> Server validates & responds with Token
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { error = "Email and Password are required." });

        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null || !PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { error = "Invalid email or password." });
        }

        if (!user.IsActive)
            return StatusCode(StatusCodes.Status403Forbidden, new { error = "User account has been deactivated." });

        var token = _jwtTokenService.GenerateToken(user);

        return Ok(new
        {
            token,
            tokenType = "Bearer",
            user = new
            {
                id = user.Id,
                email = user.Email,
                fullName = $"{user.FirstName} {user.LastName}",
                role = user.Role,
                isActive = user.IsActive
            }
        });
    }

    /// <summary>
    /// Register a new user via API
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { error = "Email and Password are required." });

        var existing = await _userRepository.GetByEmailAsync(request.Email);
        if (existing != null)
            return Conflict(new { error = "A user with this email already exists." });

        var newUser = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email.Trim().ToLower(),
            Role = string.IsNullOrWhiteSpace(request.Role) ? "Alumni" : request.Role,
            PasswordHash = PasswordHasher.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _userRepository.AddAsync(newUser);
        var token = _jwtTokenService.GenerateToken(newUser);

        return StatusCode(StatusCodes.Status201Created, new
        {
            message = "User registered successfully.",
            token,
            user = new
            {
                id = newUser.Id,
                email = newUser.Email,
                fullName = $"{newUser.FirstName} {newUser.LastName}",
                role = newUser.Role
            }
        });
    }

    /// <summary>
    /// Whiteboard Client requests with token -> Server responds with user profile
    /// </summary>
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirst("userId")?.Value;
        if (!int.TryParse(userIdStr, out var userId))
            return Unauthorized(new { error = "Invalid token claims." });

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return NotFound(new { error = "User not found." });

        return Ok(new
        {
            id = user.Id,
            email = user.Email,
            firstName = user.FirstName,
            lastName = user.LastName,
            role = user.Role,
            isActive = user.IsActive,
            createdAt = user.CreatedAt
        });
    }
}

public class LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class RegisterRequestDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string? Role { get; set; } = "Alumni";
}
