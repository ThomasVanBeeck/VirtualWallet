using Microsoft.EntityFrameworkCore;
using VirtualWallet.Interfaces;

namespace VirtualWallet.Repositories;
using VirtualWallet.Models;

public class StockRepository: AbstractBaseRepository<Stock>, IStockRepository
{
    
    public StockRepository(AppDbContext context): base(context)
    { }

    public async Task<Stock?> GetByNameAsync(string name)
    {
        return await DbSet
            .Where(s => s.StockName == name)
            .FirstOrDefaultAsync();
    }
}