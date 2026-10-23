using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Konekto.DataAccess.Context;
using Konekto.Domain.Entities;

namespace Konekto.API.Controllers.Api;

[ApiController]
[Route("api/job-postings")]
public class JobPostingsController : ControllerBase
{
    private readonly KonektoDbContext _context;

    public JobPostingsController(KonektoDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAll()
    {
        var postings = await _context.JobPostings
            .Include(j => j.Company)
            .Where(j => j.IsActive)
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

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetById(int id)
    {
        var posting = await _context.JobPostings
            .Include(j => j.Company)
            .Include(j => j.PostedByUser)
            .FirstOrDefaultAsync(j => j.Id == id);

        if (posting == null)
            return NotFound(new { message = $"İş ilanı bulunamadı (ID: {id})" });

        return Ok(new
        {
            posting.Id,
            posting.Title,
            posting.Description,
            posting.EmploymentType,
            posting.Location,
            posting.CreatedAt,
            posting.IsActive,
            Company = posting.Company == null ? null : new { posting.Company.Id, posting.Company.Name, posting.Company.Industry, posting.Company.Location },
            PostedBy = posting.PostedByUser == null ? null : new { posting.PostedByUser.Id, Name = $"{posting.PostedByUser.FirstName} {posting.PostedByUser.LastName}".Trim() }
        });
    }

    [HttpPost]
    public async Task<ActionResult<object>> Create([FromBody] JobPostingCreateDto dto)
    {
        var posting = new JobPosting
        {
            Title = dto.Title,
            Description = dto.Description,
            EmploymentType = dto.EmploymentType,
            Location = dto.Location,
            CompanyId = dto.CompanyId,
            PostedByUserId = dto.PostedByUserId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.JobPostings.Add(posting);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = posting.Id }, posting);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] JobPostingCreateDto dto)
    {
        var posting = await _context.JobPostings.FindAsync(id);
        if (posting == null)
            return NotFound(new { message = $"İş ilanı bulunamadı (ID: {id})" });

        posting.Title = dto.Title;
        posting.Description = dto.Description;
        posting.EmploymentType = dto.EmploymentType;
        posting.Location = dto.Location;
        posting.CompanyId = dto.CompanyId;

        await _context.SaveChangesAsync();
        return Ok(new { posting.Id, posting.Title, posting.EmploymentType, posting.Location });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var posting = await _context.JobPostings.FindAsync(id);
        if (posting == null)
            return NotFound(new { message = $"İş ilanı bulunamadı (ID: {id})" });

        posting.IsActive = false; // Soft delete
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // ==========================================
    // NESTED APPLICATION ROUTES
    // ==========================================

    [HttpGet("{jobPostingId}/applications")]
    public async Task<ActionResult<IEnumerable<object>>> GetApplications(int jobPostingId)
    {
        var exists = await _context.JobPostings.AnyAsync(j => j.Id == jobPostingId);
        if (!exists)
            return NotFound(new { message = $"İş ilanı bulunamadı (ID: {jobPostingId})" });

        var applications = await _context.Applications
            .Include(a => a.Alumni)
            .Where(a => a.JobPostingId == jobPostingId)
            .OrderByDescending(a => a.AppliedAt)
            .Select(a => new
            {
                a.Id,
                a.JobPostingId,
                a.AlumniId,
                a.CoverLetter,
                a.Status,
                a.AppliedAt,
                Applicant = a.Alumni == null ? null : new { a.Alumni.Id, a.Alumni.Name }
            })
            .ToListAsync();

        return Ok(applications);
    }

    [HttpPost("{jobPostingId}/applications")]
    public async Task<ActionResult<object>> CreateApplication(int jobPostingId, [FromBody] ApplicationCreateDto dto)
    {
        var posting = await _context.JobPostings.FindAsync(jobPostingId);
        if (posting == null)
            return NotFound(new { message = $"İş ilanı bulunamadı (ID: {jobPostingId})" });

        var app = new Application
        {
            JobPostingId = jobPostingId,
            AlumniId = dto.AlumniId,
            CoverLetter = dto.CoverLetter,
            Status = "Pending",
            AppliedAt = DateTime.UtcNow
        };

        _context.Applications.Add(app);
        await _context.SaveChangesAsync();

        return Created($"/api/job-postings/{jobPostingId}/applications/{app.Id}", app);
    }
}

public class JobPostingCreateDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? EmploymentType { get; set; } = "Full-time";
    public string? Location { get; set; }
    public int? CompanyId { get; set; }
    public int? PostedByUserId { get; set; }
}

public class ApplicationCreateDto
{
    public int AlumniId { get; set; }
    public string? CoverLetter { get; set; }
}

