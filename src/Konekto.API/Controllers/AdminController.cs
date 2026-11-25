using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Konekto.DataAccess.Context;
using Konekto.Domain.Entities;
using Konekto.Domain.Interfaces;

namespace Konekto.API.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly KonektoDbContext _db;
    private readonly IUserRepository _userRepository;

    public AdminController(KonektoDbContext db, IUserRepository userRepository)
    {
        _db = db;
        _userRepository = userRepository;
    }

    // GET: /Admin
    public async Task<IActionResult> Index()
    {
        var allUsers = await _userRepository.GetAllAsync();
        var allAnnouncements = await _db.Announcements.OrderByDescending(a => a.CreatedAt).ToListAsync();
        var allJobs = await _db.JobPostings.Include(j => j.Company).OrderByDescending(j => j.CreatedAt).ToListAsync();

        ViewBag.TotalUsers = allUsers.Count();
        ViewBag.AdminCount = allUsers.Count(u => u.Role == "Admin");
        ViewBag.AlumniCount = allUsers.Count(u => u.Role == "Alumni");
        ViewBag.StudentCount = allUsers.Count(u => u.Role == "Student");
        ViewBag.TotalAnnouncements = allAnnouncements.Count;
        ViewBag.TotalJobs = allJobs.Count;

        ViewBag.Announcements = allAnnouncements;
        ViewBag.Jobs = allJobs;

        return View(allUsers);
    }

    // POST: /Admin/ChangeRole -> Assigns role (Admin, Alumni, Student) to user
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeRole(int userId, string newRole)
    {
        var validRoles = new[] { "Admin", "Alumni", "Student" };
        if (!validRoles.Contains(newRole))
        {
            TempData["Error"] = "Geçersiz kullanıcı rolü seçildi.";
            return RedirectToAction(nameof(Index));
        }

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            TempData["Error"] = "Kullanıcı bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        var oldRole = user.Role;
        user.Role = newRole;
        await _userRepository.UpdateAsync(user);

        TempData["Success"] = $"{user.FirstName} {user.LastName} kullanıcısının rolü '{oldRole}' -> '{newRole}' olarak başarıyla güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Admin/ToggleStatus -> Activates or Deactivates user
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            TempData["Error"] = "Kullanıcı bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        user.IsActive = !user.IsActive;
        await _userRepository.UpdateAsync(user);

        var statusText = user.IsActive ? "aktifleştirildi" : "donduruldu";
        TempData["Success"] = $"{user.FirstName} {user.LastName} hesabı başarıyla {statusText}.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Admin/DeleteUser -> Deletes user
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            TempData["Error"] = "Kullanıcı bulunamadı.";
            return RedirectToAction(nameof(Index));
        }

        // Prevent admin from deleting themselves
        var currentUserIdStr = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (currentUserIdStr == userId.ToString())
        {
            TempData["Error"] = "Kendi oturum açtığınız admin hesabını silemezsiniz.";
            return RedirectToAction(nameof(Index));
        }

        await _userRepository.DeleteAsync(userId);
        TempData["Success"] = $"{user.FirstName} {user.LastName} kullanıcısı sistemden silindi.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Admin/DeleteAnnouncement -> Moderates announcements
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAnnouncement(int id)
    {
        var announcement = await _db.Announcements.FindAsync(id);
        if (announcement != null)
        {
            _db.Announcements.Remove(announcement);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"'{announcement.Title}' başlıklı duyuru silindi.";
        }
        return RedirectToAction(nameof(Index));
    }

    // POST: /Admin/DeleteJob -> Moderates jobs
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteJob(int id)
    {
        var job = await _db.JobPostings.FindAsync(id);
        if (job != null)
        {
            _db.JobPostings.Remove(job);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"'{job.Title}' başlıklı iş ilanı silindi.";
        }
        return RedirectToAction(nameof(Index));
    }
}
