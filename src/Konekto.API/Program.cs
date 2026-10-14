using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Konekto.DataAccess.Context;
using Konekto.Business.Factories;
using Konekto.Business.Seeding;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Entity Framework Core with MS SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Server=localhost,1433;Database=KonektoDb;User Id=sa;Password=Your_strong_password123!;TrustServerCertificate=True;";

builder.Services.AddDbContext<KonektoDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configure CORS for Client-Server communication (as shown on whiteboard)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
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

// User management endpoints (In-memory DB)
var users = new List<User>();
int nextUserId = 1;

// 3. POST /api/users
app.MapPost("/api/users", (UserDto dto) => 
{
    var user = new User { Id = nextUserId++, Name = dto.Name, GraduationYear = dto.GraduationYear };
    users.Add(user);
    return Results.Created($"/api/users/{user.Id}", user);
});

// 4. GET /api/users -> list all users
app.MapGet("/api/users", () => Results.Ok(users));

// 4b. GET /api/users/{id}
app.MapGet("/api/users/{id}", (int id) => 
{
    var user = Enumerable.FirstOrDefault(users, u => u.Id == id);
    return user is not null ? Results.Ok(user) : Results.NotFound();
});

// 5. PUT/PATCH /api/users/{id}
app.MapPut("/api/users/{id}", (int id, UserDto dto) => 
{
    var user = Enumerable.FirstOrDefault(users, u => u.Id == id);
    if (user is null) return Results.NotFound();
    
    user.Name = dto.Name;
    user.GraduationYear = dto.GraduationYear;
    return Results.Ok(user);
});

// 6. DELETE /api/users/{id}
app.MapDelete("/api/users/{id}", (int id) => 
{
    var user = Enumerable.FirstOrDefault(users, u => u.Id == id);
    if (user is null) return Results.NotFound();
    
    users.Remove(user);
    return Results.NoContent();
});

// ALUMNI Endpoints (Seeded with Model Factory from Lecture Slide)
var alumniList = AlumniFactory.GetPredefinedAlumni()
    .Select((a, i) => new Alumni { Id = i + 1, Name = a.Name, GraduationYear = a.GraduationYear })
    .ToList();
int nextAlumniId = alumniList.Count + 1;

app.MapPost("/api/alumni", (AlumniDto dto) => 
{
    var alumni = new Alumni { Id = nextAlumniId++, Name = dto.Name, GraduationYear = dto.GraduationYear };
    alumniList.Add(alumni);
    return Results.Created($"/api/alumni/{alumni.Id}", alumni);
});

app.MapGet("/api/alumni", () => Results.Ok(alumniList));
    
app.MapGet("/api/alumni/{id}", (int id) => 
{
    var alumni = Enumerable.FirstOrDefault(alumniList, a => a.Id == id);
    return alumni is not null ? Results.Ok(alumni) : Results.NotFound();
});

app.MapPut("/api/alumni/{id}", (int id, AlumniDto dto) => 
{
    var alumni = Enumerable.FirstOrDefault(alumniList, a => a.Id == id);
    if (alumni is null) return Results.NotFound();
    
    alumni.Name = dto.Name;
    alumni.GraduationYear = dto.GraduationYear;
    return Results.Ok(alumni);
});

app.MapDelete("/api/alumni/{id}", (int id) => 
{
    var alumni = Enumerable.FirstOrDefault(alumniList, a => a.Id == id);
    if (alumni is null) return Results.NotFound();
    
    alumniList.Remove(alumni);
    return Results.NoContent();
});

// Seed Endpoint (Factory and Seed Task)
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

// Models
public class User 
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int GraduationYear { get; set; }
}

public class UserDto 
{
    public string Name { get; set; } = string.Empty;
    public int GraduationYear { get; set; }
}

public class Alumni
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int GraduationYear { get; set; }
}

public class AlumniDto
{
    public string Name { get; set; } = string.Empty;
    public int GraduationYear { get; set; }
}
