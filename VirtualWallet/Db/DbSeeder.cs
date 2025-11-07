using VirtualWallet.Models;
using VirtualWallet.Repositories;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        // Optioneel: eerst de database resetten
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        // Voeg standaard users toe
        if (!db.Users.Any())
        {
            db.Users.AddRange(
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "testuser1",
                    FirstName = "Test",
                    LastName = "User1",
                    Email = "test1@example.com",
                    PasswordHash = "password1hashed" // dev placeholder
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "testuser2",
                    FirstName = "Test",
                    LastName = "User2",
                    Email = "test2@example.com",
                    PasswordHash = "password2hashed" // dev placeholder
                }
            );
        }

        db.SaveChanges();

        // Voeg andere standaard data toe
    }
}