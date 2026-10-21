namespace Konekto.Domain.Entities;

public class Alumni
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int GraduationYear { get; set; }
    public string? DegreeType { get; set; } = "Bachelor";
    public string? CurrentCompany { get; set; }
    public string? CurrentTitle { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? Bio { get; set; }
    public bool IsProfilePublic { get; set; } = true;

    // Foreign Keys & Navigation
    public int? DepartmentId { get; set; }
    public Department? Department { get; set; }

    public int? UserId { get; set; }
    public User? User { get; set; }

    public ICollection<EmploymentHistory> EmploymentHistories { get; set; } = new List<EmploymentHistory>();
    public ICollection<Application> Applications { get; set; } = new List<Application>();
}

