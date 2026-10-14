using Konekto.Domain.Entities;

namespace Konekto.Business.Factories;

public static class AlumniFactory
{
    public static Alumni Create(
        string name, 
        int graduationYear, 
        string? currentCompany = null, 
        string? currentTitle = null, 
        int? departmentId = null,
        int? userId = null)
    {
        return new Alumni
        {
            Name = name,
            GraduationYear = graduationYear,
            CurrentCompany = currentCompany,
            CurrentTitle = currentTitle,
            DepartmentId = departmentId,
            UserId = userId,
            DegreeType = "Bachelor",
            IsProfilePublic = true,
            LinkedInUrl = $"https://linkedin.com/in/{name.ToLower().Replace(" ", "-")}",
            Bio = $"Alumni graduated in {graduationYear}, working as {currentTitle ?? "Professional"} at {currentCompany ?? "Tech Industry"}."
        };
    }

    /// <summary>
    /// Returns the core alumni cohort shown in the course presentation slides.
    /// </summary>
    public static List<Alumni> GetPredefinedAlumni()
    {
        return new List<Alumni>
        {
            Create("Elif Kaya", 2024, "Google", "Cloud Solutions Architect", departmentId: 1),
            Create("Mert Aydın", 2021, "Microsoft", "Senior Backend Engineer", departmentId: 2),
            Create("Zeynep Arslan", 2019, "Trendyol", "Lead Product Manager", departmentId: 3),
            Create("Can Öztürk", 2023, "Getir", "Full Stack Developer", departmentId: 1)
        };
    }
}

