using Microsoft.EntityFrameworkCore;
using VirtualWallet.Interfaces;
using VirtualWallet.Models;

namespace VirtualWallet.Repositories;

public class HoldingRepository: IHoldingRepository
{
    private readonly AppDbContext _context;

    public HoldingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Holding?> GetByWalletAndStockAsync(Guid stockId, Guid walletId)
    {
        return await _context.Holdings
            .Include(h => h.Stock)
            .Include(h => h.Orders)
            .Where(h => h.StockId == stockId)
            .Where(h => h.WalletId == walletId)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Holding>> GetByWalletIdAsync(Guid walletId)
    {
        return await _context.Holdings
            .Include(h => h.Stock)
            .Include(h => h.Orders)
            .Where(h => h.WalletId == walletId)
            .ToListAsync();
    }

    public async Task<Holding?> AddAsync(Holding holding)
    {
        try
        {
            _context.Holdings.Add(holding);
            await _context.SaveChangesAsync();
            return holding;
        }
        catch
        {
            return null;
        }
    }
}