using VirtualWallet.Enums;
using VirtualWallet.Models;
using VirtualWallet.Repositories;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        db.Database.EnsureDeleted();
        db.Database.EnsureCreated();

        if (!db.Stocks.Any())
        {
            db.Stocks.AddRange(
                new Stock
                {
                    Id = Guid.NewGuid(),
                    StockName = "Nvidia",
                    Type = StockType.Stock,
                    Description = "Big Tech Chipmaker",
                    PricePerShare = 205.54f,
                    ChangePct24Hr = 4.67f
                },
                new Stock
                {
                    Id = Guid.NewGuid(),
                    StockName = "Bitcoin",
                    Type = StockType.Crypto,
                    Description = "The GOAT of Crypto",
                    PricePerShare = 89555.23f,
                    ChangePct24Hr = -2.67f
                },
                new Stock
                {
                    Id = Guid.NewGuid(),
                    StockName = "Tesla",
                    Type = StockType.Stock,
                    Description = "AI, Cars & Robots",
                    PricePerShare = 415.33f,
                    ChangePct24Hr = 1.23f
                },
                new Stock
                {
                    Id = Guid.NewGuid(),
                    StockName = "Ethereum",
                    Type = StockType.Crypto,
                    Description = "Decentralized Blockchain",
                    PricePerShare = 2989.66f,
                    ChangePct24Hr = -4.32f
                });
        }
        
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
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password111!")
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "testuser2",
                    FirstName = "Test",
                    LastName = "User2",
                    Email = "test2@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("password222!")
                }
            );
        }

        db.SaveChanges();
    }
}