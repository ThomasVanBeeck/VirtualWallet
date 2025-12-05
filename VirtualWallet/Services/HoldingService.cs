using VirtualWallet.Models;
using VirtualWallet.Repositories;

namespace VirtualWallet.Services;

public class HoldingService
{
    private readonly HoldingRepository _holdingRepository;

    public HoldingService(HoldingRepository holdingRepository)
    {
        _holdingRepository = holdingRepository;
    }

    public async Task<Holding?> GetHoldingByWalletIdAndStockIdAsync(Guid stockId, Guid walletId)
    {
        return await _holdingRepository.GetHoldingByWalletAndStockAsync(stockId, walletId);
    }

    public async Task<Holding?> AddHoldingAsync(Guid stockId, Guid walletId)
    {
        Holding holding = new Holding()
        {
            Id = Guid.NewGuid(),
            StockId = stockId,
            WalletId = walletId
        };
        
        return await _holdingRepository.AddAsync(holding);
    }
}