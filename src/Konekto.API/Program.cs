using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Konekto.DataAccess.Context;
using Konekto.DataAccess.Repositories;
using Konekto.Domain.Entities;
using Konekto.Domain.Interfaces;
using Konekto.Business.Factories;
using Konekto.Business.Seeding;
using ApplicationEntity = Konekto.Domain.Entities.Application;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Entity Framework Core with MS SQL Server Connection
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=localhost,1433;Database=KonektoDb;User Id=sa;Password=Your_strong_password123!;TrustServerCertificate=True;";

builder.Services.AddDbContext<KonektoDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register CRUD Database Repositories (Interfaces with DB Connection)
builder.Services.AddScoped<IAlumniRepository, AlumniRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Configure CORS for Client-Server communication (as shown on whiteboard)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseHttpsRedirection();

// Basic endpoints
app.MapGet("/hello", () => "Hello, World!");
app.MapGet("/hello/{name}", (string name) => $"Hello, {name}!");
app.MapGet("/sum/{number1}/{number2}", (int number1, int number2) => number1 + number2);

// Whiteboard Tasks:
// 1. GET /api/health -> JSON
app.MapGet("/api/health", () => Results.Json(new { status = "Healthy", timestamp = DateTime.UtcNow }));

// 7. swagger GET /api/swagger
app.MapGet("/api/swagger", () => Results.Redirect("/swagger"));

// In-Memory Fallback Stores
var fallbackUsers = new List<User>();
int nextFallbackUserId = 1;

var fallbackAlumni = AlumniFactory.GetPredefinedAlumni()
    .Select((a, i) => new Alumni { Id = i + 1, Name = a.Name, GraduationYear = a.GraduationYear })
    .ToList();
int nextFallbackAlumniId = fallbackAlumni.Count + 1;

// ========================================================
// 1. USER CRUD INTERFACES (Connected with Database)
// ========================================================

// POST /api/users
app.MapPost("/api/users", async (UserDto dto, IUserRepository repo) => 
{
    var user = new User 
    { 
        FirstName = dto.Name, 
        LastName = string.Empty, 
        Email = $"{dto.Name.ToLower().Replace(" ", ".")}@alumni.edu",
        Role = "Alumni",
        PasswordHash = "HashedPasswordSample"
    };

    try
    {
        var created = await repo.AddAsync(user);
        return Results.Created($"/api/users/{created.Id}", new { created.Id, Name = $"{created.FirstName} {created.LastName}".Trim(), dto.GraduationYear });
    }
    catch
    {
        user.Id = nextFallbackUserId++;
        fallbackUsers.Add(user);
        return Results.Created($"/api/users/{user.Id}", new { user.Id, Name = user.FirstName, dto.GraduationYear });
    }
});

// GET /api/users -> List all users
app.MapGet("/api/users", async (IUserRepository repo) => 
{
    try
    {
        var users = await repo.GetAllAsync();
        return Results.Ok(users.Select(u => new { u.Id, Name = $"{u.FirstName} {u.LastName}".Trim(), u.Email, u.Role }));
    }
    catch
    {
        return Results.Ok(fallbackUsers.Select(u => new { u.Id, Name = u.FirstName, u.Email, u.Role }));
    }
});

// GET /api/users/{id} -> Get user by ID
app.MapGet("/api/users/{id}", async (int id, IUserRepository repo) => 
{
    try
    {
        var user = await repo.GetByIdAsync(id);
        return user is not null 
            ? Results.Ok(new { user.Id, Name = $"{user.FirstName} {user.LastName}".Trim(), user.Email, user.Role }) 
            : Results.NotFound();
    }
    catch
    {
        var user = Enumerable.FirstOrDefault(fallbackUsers, u => u.Id == id);
        return user is not null ? Results.Ok(new { user.Id, Name = user.FirstName, user.Email, user.Role }) : Results.NotFound();
    }
});

// PUT /api/users/{id} -> Update user
app.MapPut("/api/users/{id}", async (int id, UserDto dto, IUserRepository repo) => 
{
    try
    {
        var user = await repo.GetByIdAsync(id);
        if (user is null) return Results.NotFound();
        
        user.FirstName = dto.Name;
        await repo.UpdateAsync(user);
        return Results.Ok(new { user.Id, Name = user.FirstName, dto.GraduationYear });
    }
    catch
    {
        var user = Enumerable.FirstOrDefault(fallbackUsers, u => u.Id == id);
        if (user is null) return Results.NotFound();
        
        user.FirstName = dto.Name;
        return Results.Ok(new { user.Id, Name = user.FirstName, dto.GraduationYear });
    }
});

// DELETE /api/users/{id} -> Delete user
app.MapDelete("/api/users/{id}", async (int id, IUserRepository repo) => 
{
    try
    {
        var success = await repo.DeleteAsync(id);
        return success ? Results.NoContent() : Results.NotFound();
    }
    catch
    {
        var user = Enumerable.FirstOrDefault(fallbackUsers, u => u.Id == id);
        if (user is null) return Results.NotFound();
        
        fallbackUsers.Remove(user);
        return Results.NoContent();
    }
});

// ========================================================
// 2. ALUMNI CRUD INTERFACES (Connected with Database)
// ========================================================

// POST /api/alumni -> Create alumni
app.MapPost("/api/alumni", async (AlumniDto dto, IAlumniRepository repo) => 
{
    var alumni = AlumniFactory.Create(dto.Name, dto.GraduationYear);

    try
    {
        var created = await repo.AddAsync(alumni);
        return Results.Created($"/api/alumni/{created.Id}", new { created.Id, created.Name, created.GraduationYear });
    }
    catch
    {
        alumni.Id = nextFallbackAlumniId++;
        fallbackAlumni.Add(alumni);
        return Results.Created($"/api/alumni/{alumni.Id}", new { alumni.Id, alumni.Name, alumni.GraduationYear });
    }
});

// GET /api/alumni -> List all alumni
app.MapGet("/api/alumni", async (IAlumniRepository repo) => 
{
    try
    {
        var alumni = await repo.GetAllAsync();
        return Results.Ok(alumni.Select(a => new { a.Id, a.Name, a.GraduationYear, a.CurrentCompany, a.CurrentTitle }));
    }
    catch
    {
        return Results.Ok(fallbackAlumni.Select(a => new { a.Id, a.Name, a.GraduationYear, a.CurrentCompany, a.CurrentTitle }));
    }
});
    
// GET /api/alumni/{id} -> Get alumni by ID
app.MapGet("/api/alumni/{id}", async (int id, IAlumniRepository repo) => 
{
    try
    {
        var alumni = await repo.GetByIdAsync(id);
        return alumni is not null 
            ? Results.Ok(new { alumni.Id, alumni.Name, alumni.GraduationYear, alumni.CurrentCompany, alumni.CurrentTitle }) 
            : Results.NotFound();
    }
    catch
    {
        var alumni = Enumerable.FirstOrDefault(fallbackAlumni, a => a.Id == id);
        return alumni is not null 
            ? Results.Ok(new { alumni.Id, alumni.Name, alumni.GraduationYear, alumni.CurrentCompany, alumni.CurrentTitle }) 
            : Results.NotFound();
    }
});

// PUT /api/alumni/{id} -> Update alumni
app.MapPut("/api/alumni/{id}", async (int id, AlumniDto dto, IAlumniRepository repo) => 
{
    try
    {
        var alumni = await repo.GetByIdAsync(id);
        if (alumni is null) return Results.NotFound();
        
        alumni.Name = dto.Name;
        alumni.GraduationYear = dto.GraduationYear;
        await repo.UpdateAsync(alumni);
        return Results.Ok(new { alumni.Id, alumni.Name, alumni.GraduationYear });
    }
    catch
    {
        var alumni = Enumerable.FirstOrDefault(fallbackAlumni, a => a.Id == id);
        if (alumni is null) return Results.NotFound();
        
        alumni.Name = dto.Name;
        alumni.GraduationYear = dto.GraduationYear;
        return Results.Ok(new { alumni.Id, alumni.Name, alumni.GraduationYear });
    }
});

// DELETE /api/alumni/{id} -> Delete alumni
app.MapDelete("/api/alumni/{id}", async (int id, IAlumniRepository repo) => 
{
    try
    {
        var success = await repo.DeleteAsync(id);
        return success ? Results.NoContent() : Results.NotFound();
    }
    catch
    {
        var alumni = Enumerable.FirstOrDefault(fallbackAlumni, a => a.Id == id);
        if (alumni is null) return Results.NotFound();
        
        fallbackAlumni.Remove(alumni);
        return Results.NoContent();
    }
});

// ========================================================
// 3. JOB POSTINGS CRUD (Connected with Database)
// ========================================================

// POST /api/job-postings
app.MapPost("/api/job-postings", async (JobPostingDto dto, KonektoDbContext db) =>
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

    db.JobPostings.Add(posting);
    await db.SaveChangesAsync();
    return Results.Created($"/api/job-postings/{posting.Id}", new
    {
        posting.Id, posting.Title, posting.Description,
        posting.EmploymentType, posting.Location,
        posting.CompanyId, posting.PostedByUserId,
        posting.CreatedAt, posting.IsActive
    });
});

// GET /api/job-postings
app.MapGet("/api/job-postings", async (KonektoDbContext db) =>
{
    var postings = await db.JobPostings
        .Include(j => j.Company)
        .Where(j => j.IsActive)
        .OrderByDescending(j => j.CreatedAt)
        .Select(j => new
        {
            j.Id, j.Title, j.EmploymentType, j.Location,
            j.CreatedAt, j.IsActive,
            Company = j.Company == null ? null : new { j.Company.Id, j.Company.Name, j.Company.Industry }
        })
        .ToListAsync();
    return Results.Ok(postings);
});

// GET /api/job-postings/{id}
app.MapGet("/api/job-postings/{id}", async (int id, KonektoDbContext db) =>
{
    var posting = await db.JobPostings
        .Include(j => j.Company)
        .Include(j => j.PostedByUser)
        .FirstOrDefaultAsync(j => j.Id == id);

    if (posting is null) return Results.NotFound();

    return Results.Ok(new
    {
        posting.Id, posting.Title, posting.Description,
        posting.EmploymentType, posting.Location,
        posting.CreatedAt, posting.IsActive,
        Company = posting.Company == null ? null : new { posting.Company.Id, posting.Company.Name, posting.Company.Industry, posting.Company.Location },
        PostedBy = posting.PostedByUser == null ? null : new { posting.PostedByUser.Id, Name = $"{posting.PostedByUser.FirstName} {posting.PostedByUser.LastName}".Trim() }
    });
});

// PUT /api/job-postings/{id}
app.MapPut("/api/job-postings/{id}", async (int id, JobPostingDto dto, KonektoDbContext db) =>
{
    var posting = await db.JobPostings.FindAsync(id);
    if (posting is null) return Results.NotFound();

    posting.Title = dto.Title;
    posting.Description = dto.Description;
    posting.EmploymentType = dto.EmploymentType;
    posting.Location = dto.Location;
    posting.CompanyId = dto.CompanyId;

    await db.SaveChangesAsync();
    return Results.Ok(new { posting.Id, posting.Title, posting.EmploymentType, posting.Location, posting.IsActive });
});

// DELETE /api/job-postings/{id}
app.MapDelete("/api/job-postings/{id}", async (int id, KonektoDbContext db) =>
{
    var posting = await db.JobPostings.FindAsync(id);
    if (posting is null) return Results.NotFound();

    posting.IsActive = false; // soft delete
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// ========================================================
// 4. APPLICATIONS (Nested routes under job-postings)
// ========================================================

// POST /api/job-postings/{jobPostingId}/applications
app.MapPost("/api/job-postings/{jobPostingId}/applications", async (int jobPostingId, ApplicationDto dto, KonektoDbContext db) =>
{
    var posting = await db.JobPostings.FindAsync(jobPostingId);
    if (posting is null) return Results.NotFound(new { error = "Job posting not found" });

    var application = new ApplicationEntity
    {
        JobPostingId = jobPostingId,
        AlumniId = dto.AlumniId,
        CoverLetter = dto.CoverLetter,
        Status = "Pending",
        AppliedAt = DateTime.UtcNow
    };

    db.Applications.Add(application);
    await db.SaveChangesAsync();
    return Results.Created($"/api/job-postings/{jobPostingId}/applications/{application.Id}", new
    {
        application.Id, application.JobPostingId, application.AlumniId,
        application.Status, application.AppliedAt
    });
});

// GET /api/job-postings/{jobPostingId}/applications
app.MapGet("/api/job-postings/{jobPostingId}/applications", async (int jobPostingId, KonektoDbContext db) =>
{
    var exists = await db.JobPostings.AnyAsync(j => j.Id == jobPostingId);
    if (!exists) return Results.NotFound(new { error = "Job posting not found" });

    var applications = await db.Applications
        .Include(a => a.Alumni)
        .Where(a => a.JobPostingId == jobPostingId)
        .OrderByDescending(a => a.AppliedAt)
        .Select(a => new
        {
            a.Id, a.JobPostingId, a.AlumniId,
            a.CoverLetter, a.Status, a.AppliedAt,
            Applicant = a.Alumni == null ? null : new { a.Alumni.Id, a.Alumni.Name }
        })
        .ToListAsync();

    return Results.Ok(applications);
});

// GET /api/job-postings/{jobPostingId}/applications/{id}
app.MapGet("/api/job-postings/{jobPostingId}/applications/{id}", async (int jobPostingId, int id, KonektoDbContext db) =>
{
    var application = await db.Applications
        .Include(a => a.Alumni)
        .Include(a => a.JobPosting)
        .FirstOrDefaultAsync(a => a.Id == id && a.JobPostingId == jobPostingId);

    if (application is null) return Results.NotFound();

    return Results.Ok(new
    {
        application.Id, application.JobPostingId, application.AlumniId,
        application.CoverLetter, application.Status, application.AppliedAt,
        Applicant = application.Alumni == null ? null : new { application.Alumni.Id, application.Alumni.Name },
        JobPosting = application.JobPosting == null ? null : new { application.JobPosting.Id, application.JobPosting.Title }
    });
});

// PUT /api/job-postings/{jobPostingId}/applications/{id}
app.MapPut("/api/job-postings/{jobPostingId}/applications/{id}", async (int jobPostingId, int id, ApplicationUpdateDto dto, KonektoDbContext db) =>
{
    var application = await db.Applications
        .FirstOrDefaultAsync(a => a.Id == id && a.JobPostingId == jobPostingId);

    if (application is null) return Results.NotFound();

    application.Status = dto.Status;
    if (dto.CoverLetter is not null) application.CoverLetter = dto.CoverLetter;

    await db.SaveChangesAsync();
    return Results.Ok(new { application.Id, application.Status, application.CoverLetter });
});

// DELETE /api/job-postings/{jobPostingId}/applications/{id}
app.MapDelete("/api/job-postings/{jobPostingId}/applications/{id}", async (int jobPostingId, int id, KonektoDbContext db) =>
{
    var application = await db.Applications
        .FirstOrDefaultAsync(a => a.Id == id && a.JobPostingId == jobPostingId);

    if (application is null) return Results.NotFound();

    db.Applications.Remove(application);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// ========================================================
// 3. SEED & UTILITY ENDPOINTS
// ========================================================

// POST /api/seed -> Trigger database seeding
app.MapPost("/api/seed", async (KonektoDbContext db) =>
{
    try
    {
        await DatabaseSeeder.SeedAsync(db);
        return Results.Ok(new { message = "Database seeded successfully using Model Factories!" });
    }
    catch (Exception ex)
    {
        return Results.Ok(new { 
            message = "In-memory stores seeded successfully (Database connection not active).", 
            detail = ex.Message 
        });
    }
});

// Auto-seed Database on startup if available
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<KonektoDbContext>();
        await DatabaseSeeder.SeedAsync(db);
    }
    catch
    {
        // Suppress if DB container is not yet spun up
    }
}

app.Run();

// ========================================================
// Request DTOs
// ========================================================
public class UserDto 
{
    public string Name { get; set; } = string.Empty;
    public int GraduationYear { get; set; }
}

public class AlumniDto
{
    public string Name { get; set; } = string.Empty;
    public int GraduationYear { get; set; }
}

public class JobPostingDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? EmploymentType { get; set; } = "Full-time";
    public string? Location { get; set; }
    public int? CompanyId { get; set; }
    public int? PostedByUserId { get; set; }
}

public class ApplicationDto
{
    public int AlumniId { get; set; }
    public string? CoverLetter { get; set; }
}

public class ApplicationUpdateDto
{
    public string Status { get; set; } = "Pending";
    public string? CoverLetter { get; set; }
}
