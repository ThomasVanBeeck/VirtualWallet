using System.Security.Claims;
using VirtualWallet.Interfaces;

namespace VirtualWallet.Services;

public class AbstractBaseService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    protected IUnitOfWork UnitOfWork { get; }

    protected AbstractBaseService(IHttpContextAccessor httpContextAccessor,  IUnitOfWork unitOfWork)
    {
        _httpContextAccessor = httpContextAccessor;
        UnitOfWork = unitOfWork;
    }

    protected Guid UserId
    {
        get
        {
            var userIdString = _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
                throw new UnauthorizedAccessException("No valid logged in user found.");
            return userId;
        }
    }
}