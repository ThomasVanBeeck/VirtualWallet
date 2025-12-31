using VirtualWallet.Models;

namespace VirtualWallet.Interfaces;

public interface IStockRepository
{
    Task<Stock?> GetByNameAsync(string name);
    Task<List<Stock>> GetAllAsync();
    void UpdateAsync(Stock updatedStock);
}