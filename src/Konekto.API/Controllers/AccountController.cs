using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Konekto.API.Models;
using Konekto.API.Services;
using Konekto.Business.Security;
using Konekto.DataAccess.Context;
using Konekto.Domain.Entities;
using Konekto.Domain.Interfaces;

namespace Konekto.API.Controllers;

public class AccountController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly KonektoDbContext _db;
    private readonly JwtTokenService _jwtTokenService;

    public AccountController(
        IUserRepository userRepository,
        KonektoDbContext db,
        JwtTokenService jwtTokenService)
    {
        _userRepository = userRepository;
        _db = db;
        _jwtTokenService = jwtTokenService;
    }

    // GET: /Account/Login
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            if (User.IsInRole("Admin"))
                return RedirectToAction("Index", "Admin");
            return RedirectToAction("Index", "Home");
        }

        ViewBag.ReturnUrl = returnUrl;
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    // POST: /Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user = await _userRepository.GetByEmailAsync(model.Email);
        if (user == null || !PasswordHasher.VerifyPassword(model.Password, user.PasswordHash))
        {
            ModelState.AddModelError(string.Empty, "Geçersiz e-posta adresi veya şifre.");
            return View(model);
        }

        if (!user.IsActive)
        {
            ModelState.AddModelError(string.Empty, "Hesabınız yönetici tarafından dondurulmuştur.");
            return View(model);
        }

        // 1. Sign In via Cookie
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role),
            new("role", user.Role),
            new("userId", user.Id.ToString())
        };

        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var authProperties = new AuthenticationProperties
        {
            IsPersistent = model.RememberMe,
            ExpiresUtc = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(14) : DateTimeOffset.UtcNow.AddHours(8)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        // 2. Also issue JWT Token for Client-Server API calls and store in cookie
        var token = _jwtTokenService.GenerateToken(user);
        Response.Cookies.Append("konekto_token", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = false,
            SameSite = SameSiteMode.Lax,
            Expires = model.RememberMe ? DateTimeOffset.UtcNow.AddDays(14) : DateTimeOffset.UtcNow.AddHours(8)
        });

        TempData["Success"] = $"Hoş geldiniz, {user.FirstName}! Giriş başarılı.";

        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return Redirect(model.ReturnUrl);

        if (user.Role == "Admin")
            return RedirectToAction("Index", "Admin");

        return RedirectToAction("Index", "Home");
    }

    // GET: /Account/Register
    [HttpGet]
    public async Task<IActionResult> Register()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
            return RedirectToAction("Index", "Home");

        try
        {
            ViewBag.Departments = await _db.Departments.OrderBy(d => d.Name).ToListAsync();
        }
        catch
        {
            ViewBag.Departments = new List<Department>();
        }

        return View(new RegisterViewModel());
    }

    // POST: /Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            try { ViewBag.Departments = await _db.Departments.OrderBy(d => d.Name).ToListAsync(); } catch { }
            return View(model);
        }

        var existingUser = await _userRepository.GetByEmailAsync(model.Email);
        if (existingUser != null)
        {
            ModelState.AddModelError("Email", "Bu e-posta adresiyle kayıtlı bir hesap zaten mevcut.");
            try { ViewBag.Departments = await _db.Departments.OrderBy(d => d.Name).ToListAsync(); } catch { }
            return View(model);
        }

        // Create User entity
        var newUser = new User
        {
            FirstName = model.FirstName.Trim(),
            LastName = model.LastName.Trim(),
            Email = model.Email.Trim().ToLower(),
            Role = model.Role == "Student" ? "Student" : "Alumni", // Default to Alumni or Student
            PasswordHash = PasswordHasher.HashPassword(model.Password),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        await _userRepository.AddAsync(newUser);

        // If user is Alumni or Student, create linked Alumni record
        try
        {
            var alumni = new Alumni
            {
                UserId = newUser.Id,
                Name = $"{newUser.FirstName} {newUser.LastName}",
                GraduationYear = model.GraduationYear ?? DateTime.UtcNow.Year,
                DegreeType = model.Role == "Student" ? "Öğrenci" : "Lisans",
                CurrentCompany = model.CurrentCompany,
                CurrentTitle = model.CurrentTitle,
                DepartmentId = model.DepartmentId,
                IsProfilePublic = true
            };

            await _db.Alumni.AddAsync(alumni);
            await _db.SaveChangesAsync();
        }
        catch
        {
            // Fail gracefully if alumni table isn't accessible
        }

        TempData["Success"] = "Kayıt işleminiz başarıyla tamamlandı! Lütfen e-posta adresiniz ve şifrenizle giriş yapınız.";
        return RedirectToAction("Login");
    }

    // GET / POST: /Account/Logout
    [HttpGet]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        Response.Cookies.Delete("konekto_token");
        TempData["Success"] = "Başarıyla çıkış yapıldı.";
        return RedirectToAction("Index", "Home");
    }

    // GET: /Account/Profile (User-specific page)
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
            return RedirectToAction("Login");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return NotFound();

        var alumni = await _db.Alumni
            .Include(a => a.Department)
            .FirstOrDefaultAsync(a => a.UserId == userId);

        var model = new ProfileViewModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role,
            CreatedAt = user.CreatedAt,
            GraduationYear = alumni?.GraduationYear,
            CurrentCompany = alumni?.CurrentCompany,
            CurrentTitle = alumni?.CurrentTitle,
            LinkedInUrl = alumni?.LinkedInUrl,
            Bio = alumni?.Bio,
            DepartmentName = alumni?.Department?.Name
        };

        return View(model);
    }

    // POST: /Account/Profile
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(UpdateProfileRequest request)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId))
            return RedirectToAction("Login");

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
            return NotFound();

        user.FirstName = request.FirstName.Trim();
        user.LastName = request.LastName.Trim();
        await _userRepository.UpdateAsync(user);

        var alumni = await _db.Alumni.FirstOrDefaultAsync(a => a.UserId == userId);
        if (alumni != null)
        {
            alumni.Name = $"{user.FirstName} {user.LastName}";
            alumni.CurrentCompany = request.CurrentCompany;
            alumni.CurrentTitle = request.CurrentTitle;
            alumni.LinkedInUrl = request.LinkedInUrl;
            alumni.Bio = request.Bio;
            await _db.SaveChangesAsync();
        }

        TempData["Success"] = "Profil bilgileriniz başarıyla güncellendi.";
        return RedirectToAction("Profile");
    }

    // GET: /Account/AccessDenied
    [HttpGet]
    public IActionResult AccessDenied()
    {
        Response.StatusCode = 403;
        return View();
    }
}
