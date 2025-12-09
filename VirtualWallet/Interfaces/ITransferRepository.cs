using VirtualWallet.Models;

namespace VirtualWallet.Interfaces;

public interface ITransferRepository
{
    Task AddAsync(Transfer transfer);
    Task<PaginatedResult<Transfer>> GetByWalletIdPaginatedAsync(Guid walletId, int page, int size);
}