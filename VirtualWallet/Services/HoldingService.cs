using VirtualWallet.Repositories;

namespace VirtualWallet.Services;

public class HoldingService
{
    private readonly HoldingRepository _holdingRepository;

    public HoldingService(HoldingRepository holdingRepository)
    {
        _holdingRepository = holdingRepository;
    }
}