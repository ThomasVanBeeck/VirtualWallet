using VirtualWallet.Models;

namespace VirtualWallet.Interfaces;

public interface IWalletRepository
{
    Task<Wallet?> GetByUserIdAsync(Guid userId);
    Task AddAsync(Wallet wallet);
    Task UpdateAsync(Wallet wallet);
}