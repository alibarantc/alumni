using Microsoft.AspNetCore.Mvc;
using Konekto.Domain.Entities;
using Konekto.Domain.Interfaces;

namespace Konekto.API.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UsersController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAll()
    {
        var users = await _userRepository.GetAllAsync();
        return Ok(users.Select(u => new 
        { 
            u.Id, 
            Name = $"{u.FirstName} {u.LastName}".Trim(), 
            u.Email, 
            u.Role,
            u.CreatedAt,
            u.IsActive
        }));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetById(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return NotFound(new { message = $"Kullanıcı bulunamadı (ID: {id})" });

        return Ok(new 
        { 
            user.Id, 
            Name = $"{user.FirstName} {user.LastName}".Trim(), 
            user.Email, 
            user.Role,
            user.CreatedAt,
            user.IsActive
        });
    }

    [HttpPost]
    public async Task<ActionResult<object>> Create([FromBody] CreateUserRequest request)
    {
        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Role = string.IsNullOrWhiteSpace(request.Role) ? "Alumni" : request.Role,
            PasswordHash = "HashedSamplePassword",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var created = await _userRepository.AddAsync(user);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, new 
        { 
            created.Id, 
            Name = $"{created.FirstName} {created.LastName}".Trim(), 
            created.Email, 
            created.Role 
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserRequest request)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return NotFound(new { message = $"Kullanıcı bulunamadı (ID: {id})" });

        if (!string.IsNullOrWhiteSpace(request.FirstName)) user.FirstName = request.FirstName;
        if (!string.IsNullOrWhiteSpace(request.LastName)) user.LastName = request.LastName;
        if (!string.IsNullOrWhiteSpace(request.Role)) user.Role = request.Role;

        await _userRepository.UpdateAsync(user);
        return Ok(new { user.Id, Name = $"{user.FirstName} {user.LastName}".Trim(), user.Email, user.Role });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _userRepository.DeleteAsync(id);
        if (!success)
            return NotFound(new { message = $"Kullanıcı bulunamadı (ID: {id})" });

        return NoContent();
    }
}

public class CreateUserRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = "Alumni";
}

public class UpdateUserRequest
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Role { get; set; }
}

