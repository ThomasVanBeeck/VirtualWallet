using VirtualWallet.Models;

namespace VirtualWallet.Interfaces;

public interface IOrderRepository
{
    Task AddAsync(Order order);
    Task<PaginatedResult<Order>> GetOrdersByWalletIdAsync(Guid walletId, int page, int size);
}