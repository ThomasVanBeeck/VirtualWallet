using VirtualWallet.Models;

namespace VirtualWallet.Interfaces;

public interface IWalletRepository
{
    Task<Wallet?> GetByUserIdAsync(Guid userId);
    void AddAsync(Wallet wallet);
    void UpdateAsync(Wallet wallet);
}