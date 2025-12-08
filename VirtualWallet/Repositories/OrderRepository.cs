using Microsoft.EntityFrameworkCore;
using VirtualWallet.Models;

namespace VirtualWallet.Repositories;

public class OrderRepository
{
    private readonly AppDbContext _context;

    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Order order)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync();
    }

    public async Task<PaginatedResult<Order>> GetOrdersByWalletIdAsync(Guid walletId, int page, int size)
    {
        if (page < 1) page = 1;
        if (size < 1) size = 10;

        var query = _context.Orders
            .AsNoTracking()
            .Where(o => o.WalletId == walletId)
            .Include(o => o.Holding)
            .ThenInclude(h => h.Stock)
            .OrderByDescending(o => o.Date);
        
        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)size);

        var items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return new PaginatedResult<Order>
        {
            Items = items.ToArray(),
            CurrentPage = page,
            PageSize = size,
            TotalPages = totalPages,
            TotalItems = totalCount
        };
    }
}