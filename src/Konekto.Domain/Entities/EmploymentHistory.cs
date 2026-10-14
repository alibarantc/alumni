namespace Konekto.Domain.Entities;

public class EmploymentHistory
{
    public int Id { get; set; }
    public int AlumniId { get; set; }
    public Alumni? Alumni { get; set; }
    public string CompanyName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public bool IsCurrent { get; set; } = false;
}

