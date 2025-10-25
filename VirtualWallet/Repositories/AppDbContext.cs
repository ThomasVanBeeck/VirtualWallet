using Microsoft.EntityFrameworkCore;
using VirtualWallet.Models;

namespace VirtualWallet.Repositories;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options)
    {
    }
    
    public DbSet<Wallet> Wallets { get; set; } = null!;
}