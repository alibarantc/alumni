using Microsoft.AspNetCore.Mvc;
using Konekto.Domain.Entities;
using Konekto.Domain.Interfaces;

namespace Konekto.API.Controllers;

public class AlumniController : Controller
{
    private readonly IAlumniRepository _alumniRepository;

    public AlumniController(IAlumniRepository alumniRepository)
    {
        _alumniRepository = alumniRepository;
    }

    // GET: /Alumni
    public async Task<IActionResult> Index(string? search)
    {
        var alumni = await _alumniRepository.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(search))
        {
            alumni = alumni.Where(a => 
                a.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                (a.CurrentCompany != null && a.CurrentCompany.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                (a.CurrentTitle != null && a.CurrentTitle.Contains(search, StringComparison.OrdinalIgnoreCase))
            );
        }

        ViewBag.SearchTerm = search;
        return View(alumni);
    }

    // GET: /Alumni/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var alumni = await _alumniRepository.GetByIdAsync(id);
        if (alumni == null)
            return NotFound();

        return View(alumni);
    }
}
