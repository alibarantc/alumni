namespace Konekto.Domain.Entities;

public class Application
{
    public int Id { get; set; }
    
    public int JobPostingId { get; set; }
    public JobPosting? JobPosting { get; set; }
    
    public int AlumniId { get; set; }
    public Alumni? Alumni { get; set; }
    
    public string? CoverLetter { get; set; }
    public string Status { get; set; } = "Pending";
    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
}
