using Microsoft.EntityFrameworkCore;
using VirtualWallet.Models;

namespace VirtualWallet.Repositories;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options)
    {
    }
    
    //public DbSet<Wallet> Wallets { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Stock> Stocks { get; set; } = null!;
}