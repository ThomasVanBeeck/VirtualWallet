using Microsoft.EntityFrameworkCore;
using VirtualWallet.Dtos;
using VirtualWallet.Interfaces;
using VirtualWallet.Models;

namespace VirtualWallet.Repositories;

public class TransferRepository: AbstractBaseRepository<Transfer>, ITransferRepository
{

    public TransferRepository(AppDbContext context): base(context)
    {
    }
    
    public async Task<PaginatedResult<Transfer>> GetByWalletIdPaginatedAsync(Guid walletId, int page, int size)
    {
        if (page < 1) page = 1;
        if (size < 1) size = 10;

        var query = DbSet
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