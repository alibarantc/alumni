using Microsoft.EntityFrameworkCore;
using Konekto.Domain.Entities;

namespace Konekto.DataAccess.Context;

public class KonektoDbContext : DbContext
{
    public KonektoDbContext(DbContextOptions<KonektoDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Alumni> Alumni => Set<Alumni>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Company> Companies => Set<Company>();
    public DbSet<JobPosting> JobPostings => Set<JobPosting>();
    public DbSet<EmploymentHistory> EmploymentHistories => Set<EmploymentHistory>();
    public DbSet<Application> Applications => Set<Application>();
    public DbSet<Announcement> Announcements => Set<Announcement>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(75);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(75);
            entity.Property(e => e.Role).IsRequired().HasMaxLength(20);
        });

        // Department configuration
        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Code).IsUnique();
            entity.Property(e => e.Code).IsRequired().HasMaxLength(10);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(120);
            entity.Property(e => e.Faculty).IsRequired().HasMaxLength(150);
        });

        // Alumni configuration
        modelBuilder.Entity<Alumni>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
            entity.Property(e => e.CurrentCompany).HasMaxLength(150);
            entity.Property(e => e.CurrentTitle).HasMaxLength(100);

            entity.HasOne(e => e.Department)
                  .WithMany(d => d.Alumni)
                  .HasForeignKey(e => e.DepartmentId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.User)
                  .WithOne(u => u.Alumni)
                  .HasForeignKey<Alumni>(e => e.UserId)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Seed default departments
        modelBuilder.Entity<Department>().HasData(
            new Department { Id = 1, Code = "YBS", Name = "Yönetim Bilişim Sistemleri", Faculty = "İktisat Fakültesi" },
            new Department { Id = 2, Code = "CENG", Name = "Bilgisayar Mühendisliği", Faculty = "Mühendislik Fakültesi" },
            new Department { Id = 3, Code = "IE", Name = "Endüstri Mühendisliği", Faculty = "Mühendislik Fakültesi" }
        );

        // JobPosting configuration
        modelBuilder.Entity<JobPosting>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(5000);
            entity.Property(e => e.EmploymentType).HasMaxLength(50);
            entity.Property(e => e.Location).HasMaxLength(150);

            entity.HasOne(e => e.Company)
                  .WithMany(c => c.JobPostings)
                  .HasForeignKey(e => e.CompanyId)
                  .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(e => e.PostedByUser)
                  .WithMany()
                  .HasForeignKey(e => e.PostedByUserId)
                  .OnDelete(DeleteBehavior.SetNull);

            // Composite index for active job feed
            entity.HasIndex(e => new { e.IsActive, e.CreatedAt })
                  .HasDatabaseName("IX_JobPostings_IsActive_CreatedAt");
        });

        // Application configuration
        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Status).IsRequired().HasMaxLength(30);
            entity.Property(e => e.CoverLetter).HasMaxLength(2000);

            entity.HasOne(e => e.JobPosting)
                  .WithMany(j => j.Applications)
                  .HasForeignKey(e => e.JobPostingId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Alumni)
                  .WithMany(a => a.Applications)
                  .HasForeignKey(e => e.AlumniId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Announcement configuration
        modelBuilder.Entity<Announcement>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(250);
            entity.Property(e => e.Content).IsRequired().HasMaxLength(4000);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Author).IsRequired().HasMaxLength(100);
        });
    }
}

