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
                    Symbol = "NVDA",
                    Type = StockType.Stock,
                    Description = "Big Tech Chipmaker",
                    PricePerShare = 205.54f,
                    ChangePct24Hr = 4.67f
                },
                new Stock
                {
                    Id = Guid.NewGuid(),
                    StockName = "Bitcoin",
                    Symbol = "BTC",
                    Type = StockType.Crypto,
                    Description = "The GOAT of Crypto",
                    PricePerShare = 89555.23f,
                    ChangePct24Hr = -2.67f
                },
                new Stock
                {
                    Id = Guid.NewGuid(),
                    StockName = "Tesla",
                    Symbol = "TSLA",
                    Type = StockType.Stock,
                    Description = "AI, Cars & Robots",
                    PricePerShare = 415.33f,
                    ChangePct24Hr = 1.23f
                },
                new Stock
                {
                    Id = Guid.NewGuid(),
                    StockName = "Ethereum",
                    Symbol = "ETH",
                    Type = StockType.Crypto,
                    Description = "Decentralized Blockchain",
                    PricePerShare = 2989.66f,
                    ChangePct24Hr = -4.32f
                }
                );
        }

        var user1 = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser1",
            FirstName = "Test",
            LastName = "User1",
            Email = "test1@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password111!")
        };
        var user2 = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser2",
            FirstName = "Test",
            LastName = "User2",
            Email = "test2@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password222!")
        };
        
        if (!db.Users.Any())
            db.Users.AddRange(user1, user2);
        
        var wallet1 = new Wallet
        {
            Id = Guid.NewGuid(),
            UserId = user1.Id
        };
        var wallet2 = new Wallet
        {
            Id = Guid.NewGuid(),
            UserId = user2.Id
        };

        if (!db.Wallets.Any())
        {
            db.Wallets.AddRange(wallet1, wallet2);
        }
        db.SaveChanges();
    }
}