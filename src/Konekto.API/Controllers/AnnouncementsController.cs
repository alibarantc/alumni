using Microsoft.AspNetCore.Mvc;
using Konekto.Domain.Entities;
using Konekto.Domain.Interfaces;

namespace Konekto.API.Controllers;

public class AnnouncementsController : Controller
{
    private readonly IAnnouncementRepository _repository;

    public AnnouncementsController(IAnnouncementRepository repository)
    {
        _repository = repository;
    }

    // GET: /Announcements
    public async Task<IActionResult> Index(string? category)
    {
        var announcements = await _repository.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(category) && category != "Tümü")
        {
            announcements = announcements.Where(a => a.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
        }

        ViewBag.SelectedCategory = category ?? "Tümü";
        return View(announcements);
    }

    // GET: /Announcements/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var announcement = await _repository.GetByIdAsync(id);
        if (announcement == null)
            return NotFound();

        return View(announcement);
    }

    // GET: /Announcements/Create
    [HttpGet]
    public IActionResult Create()
    {
        return View(new Announcement());
    }

    // POST: /Announcements/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Announcement announcement)
    {
        if (string.IsNullOrWhiteSpace(announcement.Title) || string.IsNullOrWhiteSpace(announcement.Content))
        {
            ModelState.AddModelError("", "Başlık ve içerik alanları zorunludur.");
            return View(announcement);
        }

        announcement.CreatedAt = DateTime.UtcNow;
        announcement.IsActive = true;
        if (string.IsNullOrWhiteSpace(announcement.Author))
            announcement.Author = "Yönetici";

        await _repository.AddAsync(announcement);
        return RedirectToAction(nameof(Index));
    }

    // GET: /Announcements/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var announcement = await _repository.GetByIdAsync(id);
        if (announcement == null)
            return NotFound();

        await _repository.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
