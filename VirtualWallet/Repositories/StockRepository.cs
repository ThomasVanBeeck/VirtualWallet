using Microsoft.EntityFrameworkCore;

namespace VirtualWallet.Repositories;
using VirtualWallet.Models;

public class StockRepository
{
    private readonly AppDbContext _context;

    public StockRepository(AppDbContext context)
    {
        _context = context;
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