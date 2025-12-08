using Microsoft.EntityFrameworkCore;
using VirtualWallet.Models;

namespace VirtualWallet.Repositories;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) 
        : base(options)
    { }
    
    public DbSet<Holding> Holdings { get; set; } = null!;
    public DbSet<Order> Orders { get; set; } = null!;
    public DbSet<Stock> Stocks { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Wallet> Wallets { get; set; } = null!;
    public DbSet<Transfer> Transfers { get; set; } = null!;
    public DbSet<ScheduleTimer> ScheduleTimers { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Holding>()
            .HasMany(holding => holding.Orders)
            .WithOne(order => order.Holding)
            .HasForeignKey(order => order.HoldingId);
        
        modelBuilder.Entity<ScheduleTimer>()
            .HasKey(s => s.Key);
        
        modelBuilder.Entity<Stock>()
            .HasMany(stock => stock.Holdings)
            .WithOne(holding => holding.Stock)
            .HasForeignKey(holding => holding.StockId);
        
        modelBuilder.Entity<User>()
            .HasOne(user => user.Wallet)
            .WithOne(wallet => wallet.User)
            .HasForeignKey<Wallet>(wallet => wallet.UserId);

        modelBuilder.Entity<Wallet>()
            .HasMany(wallet => wallet.Holdings)
            .WithOne(holding => holding.Wallet)
            .HasForeignKey(holding => holding.WalletId);
            
        modelBuilder.Entity<Wallet>()
            .HasMany(wallet => wallet.Transfers)
            .WithOne(transfer => transfer.Wallet)
            .HasForeignKey(transfer => transfer.WalletId);

        modelBuilder.Entity<Wallet>()
            .HasMany(wallet => wallet.Orders)
            .WithOne(order => order.Wallet)
            .HasForeignKey(order => order.WalletId);

    }
}