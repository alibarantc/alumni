using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Konekto.DataAccess.Context;
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

    // =========================================================================
    // Nested Route: /api/users/{id}/job-postings (6. Hafta İsteri)
    // =========================================================================

    [HttpGet("{id}/job-postings")]
    public async Task<ActionResult<IEnumerable<object>>> GetUserJobPostings(int id, [FromServices] KonektoDbContext db)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return NotFound(new { message = $"Kullanıcı bulunamadı (ID: {id})" });

        var postings = await db.JobPostings
            .Include(j => j.Company)
            .Where(j => j.PostedByUserId == id)
            .OrderByDescending(j => j.CreatedAt)
            .Select(j => new
            {
                j.Id,
                j.Title,
                j.Description,
                j.EmploymentType,
                j.Location,
                j.CreatedAt,
                j.IsActive,
                Company = j.Company == null ? null : new { j.Company.Id, j.Company.Name, j.Company.Industry }
            })
            .ToListAsync();

        return Ok(postings);
    }

    [HttpPost("{id}/job-postings")]
    public async Task<ActionResult<object>> CreateUserJobPosting(int id, [FromBody] CreateUserJobPostingRequest request, [FromServices] KonektoDbContext db)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return NotFound(new { message = $"Kullanıcı bulunamadı (ID: {id})" });

        if (string.IsNullOrWhiteSpace(request.Title))
            return BadRequest(new { message = "Başlık (Title) alanı zorunludur." });

        if (request.Title.Length > 120)
            return BadRequest(new { message = "Başlık (Title) en fazla 120 karakter olabilir." });

        if (request.CompanyId == null || request.CompanyId <= 0)
            return BadRequest(new { message = "Şirket (CompanyId) alanı zorunludur." });

        var posting = new JobPosting
        {
            Title = request.Title,
            Description = request.Description ?? string.Empty,
            EmploymentType = request.EmploymentType,
            Location = request.Location,
            CompanyId = request.CompanyId,
            PostedByUserId = id,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        db.JobPostings.Add(posting);
        await db.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUserJobPostings), new { id = user.Id }, new
        {
            posting.Id,
            posting.Title,
            posting.Description,
            posting.EmploymentType,
            posting.Location,
            posting.CompanyId,
            posting.PostedByUserId,
            posting.CreatedAt,
            posting.IsActive
        });
    }
}

public class CreateUserJobPostingRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? EmploymentType { get; set; } = "Full-time";
    public string? Location { get; set; }
    public int? CompanyId { get; set; }
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

