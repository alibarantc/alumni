namespace Konekto.Domain.Entities;

public class Company
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Industry { get; set; }
    public string? Website { get; set; }
    public string? Location { get; set; }

    // Navigation
    public ICollection<JobPosting> JobPostings { get; set; } = new List<JobPosting>();
}
