using Konekto.Domain.Entities;

namespace Konekto.Business.Factories;

public static class UserFactory
{
    public static User Create(string firstName, string lastName, string email, string role = "Alumni")
    {
        return new User
        {
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Role = role,
            PasswordHash = "AQAAAAIAAYagAAAAEIQ7g2vX9X...hashed_seed_password_sample",
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
    }

    public static List<User> GetPredefinedUsers()
    {
        return new List<User>
        {
            Create("Admin", "System", "admin@konekto.edu", role: "Admin"),
            Create("Elif", "Kaya", "elif.kaya@alumni.edu", role: "Alumni"),
            Create("Mert", "Aydın", "mert.aydin@alumni.edu", role: "Alumni"),
            Create("Zeynep", "Arslan", "zeynep.arslan@alumni.edu", role: "Alumni"),
            Create("Can", "Öztürk", "can.ozturk@alumni.edu", role: "Alumni")
        };
    }
}
