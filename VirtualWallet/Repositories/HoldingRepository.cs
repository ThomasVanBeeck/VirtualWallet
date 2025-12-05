using Microsoft.EntityFrameworkCore;
using VirtualWallet.Models;

namespace VirtualWallet.Repositories;

public class HoldingRepository
{
    private readonly AppDbContext _context;

    public HoldingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Holding?> GetHoldingByWalletAndStockAsync(Guid stockId, Guid walletId)
    {
        return await _context.Holdings
            .Where(h => h.StockId == stockId)
            .Where(h => h.WalletId == walletId)
            .FirstOrDefaultAsync();
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