using Microsoft.EntityFrameworkCore;
using VirtualWallet.DTOs;
using VirtualWallet.Models;

namespace VirtualWallet.Repositories;

public class TransferRepository
{
    private readonly AppDbContext _context;

    public TransferRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Transfer transfer)
    {
        _context.Transfers.Add(transfer);
        await _context.SaveChangesAsync();
    }
    
    public async Task<PaginatedResult<Transfer>> GetByWalletIdPaginatedAsync(Guid walletId, int page, int size)
    {
        if (page < 1) page = 1;
        if (size < 1) size = 10;

        var query = _context.Transfers
            .AsNoTracking()
            .Where(t => t.WalletId == walletId)
            .OrderByDescending(t => t.Date);

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)size);

        var items = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return new PaginatedResult<Transfer>
        {
            Items = items.ToArray(),
            CurrentPage = page,
            PageSize = size,
            TotalPages = totalPages,
            TotalItems = totalCount
        };
    }
}