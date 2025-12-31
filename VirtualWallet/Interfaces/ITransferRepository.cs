using VirtualWallet.Models;

namespace VirtualWallet.Interfaces;

public interface ITransferRepository
{
    void AddAsync(Transfer transfer);
    Task<PaginatedResult<Transfer>> GetByWalletIdPaginatedAsync(Guid walletId, int page, int size);
}