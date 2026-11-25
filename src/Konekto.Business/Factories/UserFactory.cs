using Konekto.Domain.Entities;
using Konekto.Business.Security;

namespace Konekto.Business.Factories;

public static class UserFactory
{
    public static User Create(string firstName, string lastName, string email, string role = "Alumni", string rawPassword = "Password123!")
    {
        return new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Role = role,
            PasswordHash = PasswordHasher.HashPassword(rawPassword),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
    }

    public static List<User> GetPredefinedUsers()
    {
        return new List<User>
        {
            Create("Sistem", "Yöneticisi", "admin@konekto.edu.tr", role: "Admin", rawPassword: "Admin123!"),
            Create("Ahmet", "Yılmaz", "alumni@konekto.edu.tr", role: "Alumni", rawPassword: "Alumni123!"),
            Create("Ayşe", "Demir", "student@konekto.edu.tr", role: "Student", rawPassword: "Student123!"),
            Create("Elif", "Kaya", "elif.kaya@alumni.edu", role: "Alumni", rawPassword: "Alumni123!"),
            Create("Mert", "Aydın", "mert.aydin@alumni.edu", role: "Alumni", rawPassword: "Alumni123!"),
            Create("Zeynep", "Arslan", "zeynep.arslan@alumni.edu", role: "Alumni", rawPassword: "Alumni123!"),
            Create("Can", "Öztürk", "can.ozturk@alumni.edu", role: "Alumni", rawPassword: "Alumni123!")
        };
    }
}
