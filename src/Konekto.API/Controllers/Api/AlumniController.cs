using Microsoft.AspNetCore.Mvc;
using Konekto.Domain.Entities;
using Konekto.Domain.Interfaces;
using Konekto.Business.Factories;

namespace Konekto.API.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class AlumniController : ControllerBase
{
    private readonly IAlumniRepository _alumniRepository;

    public AlumniController(IAlumniRepository alumniRepository)
    {
        _alumniRepository = alumniRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<object>>> GetAll()
    {
        var alumni = await _alumniRepository.GetAllAsync();
        return Ok(alumni.Select(a => new 
        { 
            a.Id, 
            a.Name, 
            a.GraduationYear, 
            a.CurrentCompany, 
            a.CurrentTitle,
            Department = a.Department?.Name
        }));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<object>> GetById(int id)
    {
        var alumni = await _alumniRepository.GetByIdAsync(id);
        if (alumni == null)
            return NotFound(new { message = $"Mezun bulunamadı (ID: {id})" });

        return Ok(new 
        { 
            alumni.Id, 
            alumni.Name, 
            alumni.GraduationYear, 
            alumni.CurrentCompany, 
            alumni.CurrentTitle,
            alumni.LinkedInUrl,
            alumni.Bio,
            Department = alumni.Department?.Name
        });
    }

    [HttpPost]
    public async Task<ActionResult<object>> Create([FromBody] CreateAlumniRequest request)
    {
        var alumni = AlumniFactory.Create(request.Name, request.GraduationYear);
        alumni.CurrentCompany = request.CurrentCompany;
        alumni.CurrentTitle = request.CurrentTitle;

        var created = await _alumniRepository.AddAsync(alumni);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, new 
        { 
            created.Id, 
            created.Name, 
            created.GraduationYear, 
            created.CurrentCompany, 
            created.CurrentTitle 
        });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAlumniRequest request)
    {
        var alumni = await _alumniRepository.GetByIdAsync(id);
        if (alumni == null)
            return NotFound(new { message = $"Mezun bulunamadı (ID: {id})" });

        if (!string.IsNullOrWhiteSpace(request.Name)) alumni.Name = request.Name;
        if (request.GraduationYear.HasValue) alumni.GraduationYear = request.GraduationYear.Value;
        if (request.CurrentCompany != null) alumni.CurrentCompany = request.CurrentCompany;
        if (request.CurrentTitle != null) alumni.CurrentTitle = request.CurrentTitle;

        await _alumniRepository.UpdateAsync(alumni);
        return Ok(new { alumni.Id, alumni.Name, alumni.GraduationYear, alumni.CurrentCompany, alumni.CurrentTitle });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _alumniRepository.DeleteAsync(id);
        if (!success)
            return NotFound(new { message = $"Mezun bulunamadı (ID: {id})" });

        return NoContent();
    }
}

public class CreateAlumniRequest
{
    public string Name { get; set; } = string.Empty;
    public int GraduationYear { get; set; }
    public string? CurrentCompany { get; set; }
    public string? CurrentTitle { get; set; }
}

public class UpdateAlumniRequest
{
    public string? Name { get; set; }
    public int? GraduationYear { get; set; }
    public string? CurrentCompany { get; set; }
    public string? CurrentTitle { get; set; }
}

