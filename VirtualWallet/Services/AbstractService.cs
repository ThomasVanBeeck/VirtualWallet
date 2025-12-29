using System.Security.Claims;

namespace VirtualWallet.Services;

public class AbstractService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    protected AbstractService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
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