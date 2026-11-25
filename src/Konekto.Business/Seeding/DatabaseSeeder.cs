using Microsoft.EntityFrameworkCore;
using Konekto.DataAccess.Context;
using Konekto.Business.Factories;

namespace Konekto.Business.Seeding;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(KonektoDbContext context)
    {
        // 1. Seed Companies
        if (!await context.Companies.AnyAsync())
        {
            var companies = CompanyFactory.GetPredefinedCompanies();
            await context.Companies.AddRangeAsync(companies);
            await context.SaveChangesAsync();
        }

        // 2. Seed Users
        if (!await context.Users.AnyAsync())
        {
            var users = UserFactory.GetPredefinedUsers();
            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }
        else
        {
            // Ensure admin@konekto.edu.tr exists
            var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Email == "admin@konekto.edu.tr");
            if (adminUser == null)
            {
                var admin = UserFactory.Create("Sistem", "Yöneticisi", "admin@konekto.edu.tr", role: "Admin", rawPassword: "Admin123!");
                await context.Users.AddAsync(admin);
                await context.SaveChangesAsync();
            }
        }

        // 3. Seed Alumni Records
        if (!await context.Alumni.AnyAsync())
        {
            var alumni = AlumniFactory.GetPredefinedAlumni();
            await context.Alumni.AddRangeAsync(alumni);
            await context.SaveChangesAsync();
        }

        // 4. Seed Job Postings
        if (!await context.JobPostings.AnyAsync())
        {
            var jobPostings = JobPostingFactory.GetPredefinedJobPostings();
            await context.JobPostings.AddRangeAsync(jobPostings);
            await context.SaveChangesAsync();
        }

        // 5. Seed Announcements
        if (!await context.Announcements.AnyAsync())
        {
            var announcements = AnnouncementFactory.GetPredefinedAnnouncements();
            await context.Announcements.AddRangeAsync(announcements);
            await context.SaveChangesAsync();
        }
    }
}
