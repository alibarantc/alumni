using Microsoft.AspNetCore.Mvc;
using Konekto.Domain.Entities;
using Konekto.Domain.Interfaces;

namespace Konekto.API.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class AnnouncementsController : ControllerBase
{
    private readonly IAnnouncementRepository _repository;

    public AnnouncementsController(IAnnouncementRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Announcement>>> GetAll()
    {
        var announcements = await _repository.GetAllAsync();
        return Ok(announcements);
    }

    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<Announcement>>> GetActive()
    {
        var announcements = await _repository.GetActiveAsync();
        return Ok(announcements);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Announcement>> GetById(int id)
    {
        var announcement = await _repository.GetByIdAsync(id);
        if (announcement == null)
            return NotFound(new { message = $"Duyuru bulunamadı (ID: {id})" });

        return Ok(announcement);
    }

    [HttpPost]
    public async Task<ActionResult<Announcement>> Create([FromBody] Announcement announcement)
    {
        if (string.IsNullOrWhiteSpace(announcement.Title) || string.IsNullOrWhiteSpace(announcement.Content))
            return BadRequest(new { message = "Başlık ve içerik alanları zorunludur." });

        announcement.CreatedAt = DateTime.UtcNow;
        var created = await _repository.AddAsync(announcement);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Announcement announcement)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            return NotFound(new { message = $"Duyuru bulunamadı (ID: {id})" });

        existing.Title = announcement.Title;
        existing.Content = announcement.Content;
        existing.Category = announcement.Category;
        existing.Author = announcement.Author;
        existing.IsActive = announcement.IsActive;

        await _repository.UpdateAsync(existing);
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _repository.DeleteAsync(id);
        if (!success)
            return NotFound(new { message = $"Duyuru bulunamadı (ID: {id})" });

        return NoContent();
    }
}

