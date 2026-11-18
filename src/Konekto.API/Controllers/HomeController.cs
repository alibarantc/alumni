using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Konekto.Domain.Entities;
using Konekto.Domain.Interfaces;
using Konekto.DataAccess.Context;

namespace Konekto.API.Controllers;

public class HomeController : Controller
{
    private readonly IAnnouncementRepository _announcementRepo;
    private readonly IAlumniRepository _alumniRepo;
    private readonly KonektoDbContext _db;

    public HomeController(
        IAnnouncementRepository announcementRepo, 
        IAlumniRepository alumniRepo,
        KonektoDbContext db)
    {
        _announcementRepo = announcementRepo;
        _alumniRepo = alumniRepo;
        _db = db;
    }

    public async Task<IActionResult> Index()
    {
        var recentAnnouncements = (await _announcementRepo.GetActiveAsync()).Take(3).ToList();
        var allAlumni = (await _alumniRepo.GetAllAsync()).ToList();

        ViewBag.RecentAnnouncements = recentAnnouncements;
        ViewBag.TotalAlumniCount = allAlumni.Count;
        ViewBag.ActiveAnnouncementsCount = (await _announcementRepo.GetActiveAsync()).Count();
        ViewBag.FeaturedAlumni = allAlumni.Take(4).ToList();

        try
        {
            ViewBag.RecentJobs = await _db.JobPostings
                .Include(j => j.Company)
                .Where(j => j.IsActive)
                .OrderByDescending(j => j.CreatedAt)
                .Take(3)
                .ToListAsync();

            ViewBag.TotalJobsCount = await _db.JobPostings.CountAsync(j => j.IsActive);
            ViewBag.TotalCompaniesCount = await _db.Companies.CountAsync();
        }
        catch
        {
            ViewBag.RecentJobs = new List<JobPosting>();
            ViewBag.TotalJobsCount = 0;
            ViewBag.TotalCompaniesCount = 0;
        }

        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    [HttpGet("not-found")]
    [HttpGet("404")]
    [ActionName("NotFound")]
    public IActionResult PageNotFound()
    {
        Response.StatusCode = 404;
        return View("NotFound");
    }
}
