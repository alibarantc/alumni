using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    app.MapOpenApi();
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

// ALUMNI Endpoints (From the Presentation Slide)
var alumniList = new List<Alumni>();
int nextAlumniId = 1;

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
