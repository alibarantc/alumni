namespace Konekto.Domain.Entities;

public class Department
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Faculty { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<Alumni> Alumni { get; set; } = new List<Alumni>();
}

