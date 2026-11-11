using Konekto.Domain.Entities;

namespace Konekto.Business.Factories;

public static class JobPostingFactory
{
    public static JobPosting Create(string title, string description, string employmentType, string location, int companyId, int? postedByUserId = null)
    {
        return new JobPosting
        {
            Title = title,
            Description = description,
            EmploymentType = employmentType,
            Location = location,
            CompanyId = companyId,
            PostedByUserId = postedByUserId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
    }

    public static List<JobPosting> GetPredefinedJobPostings(IReadOnlyList<int> companyIds)
    {
        if (companyIds.Count == 0)
            throw new ArgumentException("At least one seeded company is required.", nameof(companyIds));

        return new List<JobPosting>
        {
            Create(
                "Backend Developer",
                "We are looking for a .NET Core backend developer with experience in REST APIs, EF Core and SQL Server. You will work in an agile team building scalable microservices.",
                "Full-time",
                "Istanbul, Turkey / Remote",
                companyIds[0 % companyIds.Count]
            ),
            Create(
                "Frontend Developer",
                "Join our product team as a React/Next.js frontend developer. You will collaborate with designers and backend engineers to build modern web applications.",
                "Full-time",
                "Istanbul, Turkey",
                companyIds[1 % companyIds.Count]
            ),
            Create(
                "Data Science Intern",
                "Summer internship opportunity for students interested in machine learning, Python, and data pipelines. Work on real-world datasets with senior data scientists.",
                "Internship",
                "Istanbul, Turkey",
                companyIds[2 % companyIds.Count]
            ),
            Create(
                "DevOps Engineer",
                "We need an experienced DevOps/SRE engineer to manage our Kubernetes clusters, CI/CD pipelines (GitHub Actions) and cloud infrastructure (Azure).",
                "Full-time",
                "Remote",
                companyIds[3 % companyIds.Count]
            ),
            Create(
                "Product Manager",
                "Seeking a product manager to drive roadmap planning, stakeholder alignment, and sprint execution for our alumni networking platform.",
                "Full-time",
                "Istanbul, Turkey",
                companyIds[4 % companyIds.Count]
            ),
            Create(
                "Mobile Developer (React Native)",
                "Part-time mobile developer role for our cross-platform alumni mobile app. Experience with React Native, Expo, and REST API integration required.",
                "Part-time",
                "Remote",
                companyIds[5 % companyIds.Count]
            )
        };
    }
}
