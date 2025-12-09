using VirtualWallet.Models;

namespace VirtualWallet.Interfaces;

public interface IHoldingRepository
{
    Task<Holding?> GetByWalletAndStockAsync(Guid stockId, Guid walletId);
    Task<List<Holding>> GetByWalletIdAsync(Guid walletId);
    Task<Holding?> AddAsync(Holding holding);
}