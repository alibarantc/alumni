namespace Konekto.Domain.Entities;

public class JobPosting
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? EmploymentType { get; set; } = "Full-time";
    public string? Location { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Foreign Keys & Navigation
    public int? CompanyId { get; set; }
    public Company? Company { get; set; }

    public int? PostedByUserId { get; set; }
    public User? PostedByUser { get; set; }

    public ICollection<Application> Applications { get; set; } = new List<Application>();
}

