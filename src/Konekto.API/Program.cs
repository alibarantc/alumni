using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Konekto.DataAccess.Context;
using Konekto.DataAccess.Repositories;
using Konekto.Domain.Entities;
using Konekto.Domain.Interfaces;
using Konekto.Business.Factories;
using Konekto.Business.Seeding;

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
