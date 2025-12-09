using VirtualWallet.Interfaces;

namespace VirtualWallet.Services;

public class HoldingService
{
    private readonly IHoldingRepository _holdingRepository;

    public HoldingService(IHoldingRepository holdingRepository)
    {
        _holdingRepository = holdingRepository;
    }
}