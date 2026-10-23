using Microsoft.AspNetCore.Mvc;
using Konekto.Domain.Interfaces;

namespace Konekto.API.Controllers;

public class HomeController : Controller
{
    private readonly IAnnouncementRepository _announcementRepo;
    private readonly IAlumniRepository _alumniRepo;

    public HomeController(IAnnouncementRepository announcementRepo, IAlumniRepository alumniRepo)
    {
        _announcementRepo = announcementRepo;
        _alumniRepo = alumniRepo;
    }

    public async Task<IActionResult> Index()
    {
        var recentAnnouncements = (await _announcementRepo.GetActiveAsync()).Take(4).ToList();
        var allAlumni = await _alumniRepo.GetAllAsync();

        ViewBag.RecentAnnouncements = recentAnnouncements;
        ViewBag.TotalAlumniCount = allAlumni.Count();
        ViewBag.ActiveAnnouncementsCount = (await _announcementRepo.GetActiveAsync()).Count();

        return View();
    }

    public IActionResult About()
    {
        return View();
    }
}

