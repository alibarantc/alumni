using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Konekto.DataAccess.Context;
using Konekto.DataAccess.Repositories;
using Konekto.Domain.Interfaces;
using Konekto.Business.Seeding;
using Konekto.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
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
builder.Services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();

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

// Custom Terminal Logger Middleware (Format: time method route restype duration)
app.UseRequestLogging();

// 5xx Guardian Middleware (Catches unhandled exceptions and returns structured JSON on 500 errors)
app.Use5xxGuardian();

// 404 Guardian Middleware (Returns structured JSON with reason for 404 errors)
app.UseNotFoundGuardian();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseStaticFiles();

// Map MVC Controllers & API Controllers
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Basic endpoints
app.MapGet("/hello", () => "Hello, World!");
app.MapGet("/hello/{name}", (string name) => $"Hello, {name}!");
app.MapGet("/sum/{number1}/{number2}", (int number1, int number2) => number1 + number2);

// Whiteboard Tasks:
// 1. GET /api/health -> JSON
app.MapGet("/api/health", () => Results.Json(new { status = "Healthy", timestamp = DateTime.UtcNow }));

// 7. swagger GET /api/swagger
app.MapGet("/api/swagger", () => Results.Redirect("/swagger"));

// Deliberately expose the 5xx test endpoint in Development only.
if (app.Environment.IsDevelopment())
{
    app.MapGet("/api/test-500", () =>
    {
        throw new InvalidOperationException("Simulated server failure for 5xx Guardian tests.");
    });
}

// Only routes without MVC Controller equivalents remain Minimal APIs.
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

// SEED & UTILITY ENDPOINTS
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
        app.Logger.LogError(ex, "Database seeding failed");
        return Results.Problem("Database seeding failed.", statusCode: StatusCodes.Status503ServiceUnavailable);
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

// DTO for the remaining nested application update endpoint.
public class ApplicationUpdateDto
{
    public string Status { get; set; } = "Pending";
    public string? CoverLetter { get; set; }
}
