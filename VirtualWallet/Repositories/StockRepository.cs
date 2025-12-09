using Microsoft.EntityFrameworkCore;
using VirtualWallet.Interfaces;

namespace VirtualWallet.Repositories;
using VirtualWallet.Models;

public class StockRepository: IStockRepository
{
    private readonly AppDbContext _context;

    public StockRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Stock?> GetByNameAsync(string name)
    {
        return await _context.Stocks
            .Where(s => s.StockName == name)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Stock>> GetAllAsync()
    {
        return await _context.Stocks.ToListAsync();
    }
    
    public async Task UpdateAsync(Stock updatedStock)
    { 
        _context.Stocks.Update(updatedStock);
        await _context.SaveChangesAsync();
    }
}