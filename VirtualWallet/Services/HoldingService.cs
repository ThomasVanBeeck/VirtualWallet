using VirtualWallet.Interfaces;

namespace VirtualWallet.Services;

public class HoldingService : AbstractService
{
    private readonly IHoldingRepository _holdingRepository;

    public HoldingService(IHoldingRepository holdingRepository,
        IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
    {
        _holdingRepository = holdingRepository;
    }
    
    // Holding service is (op dit moment) nog leeg.
}