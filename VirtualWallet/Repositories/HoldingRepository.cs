using Microsoft.EntityFrameworkCore;
using VirtualWallet.Interfaces;
using VirtualWallet.Models;

namespace VirtualWallet.Repositories;

public class HoldingRepository: AbstractBaseRepository<Holding>, IHoldingRepository
{
    
    public HoldingRepository(AppDbContext context) : base(context)
    { }

    public async Task<Holding?> GetByWalletAndStockAsync(Guid stockId, Guid walletId)
    {
        return await DbSet
            .Include(h => h.Stock)
            .Include(h => h.Orders)
            .Where(h => h.StockId == stockId)
            .Where(h => h.WalletId == walletId)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Holding>> GetByWalletIdAsync(Guid walletId)
    {
        return await DbSet
            .Include(h => h.Stock)
            .Include(h => h.Orders)
            .Where(h => h.WalletId == walletId)
            .ToListAsync();
    }
}