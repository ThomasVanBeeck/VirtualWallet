using VirtualWallet.Interfaces;

namespace VirtualWallet.Services;

public class HoldingService : AbstractBaseService
{
    private readonly IHoldingRepository _holdingRepository;

    public HoldingService(IHoldingRepository holdingRepository,
        IHttpContextAccessor httpContextAccessor,
        IUnitOfWork unitOfWork) : base(httpContextAccessor, unitOfWork)
    {
        _holdingRepository = holdingRepository;
    }
    
    // Holding service is (op dit moment) nog leeg.
}