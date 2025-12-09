using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtualWallet.Dtos;
using VirtualWallet.Services;

namespace VirtualWallet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletController : ControllerBase
{
    private readonly WalletService _walletService;

    public WalletController(WalletService walletService)
    {
        _walletService = walletService;
    }

    [Authorize(AuthenticationSchemes = "CookieAuth")]
    [HttpGet]
    public async Task<ActionResult<WalletSummaryDto>> GetWalletSummary(
        [FromQuery] int page = 1, [FromQuery] int size = 1)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userIdString))
            return Unauthorized(); 
        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized();
        
        var walletSummaryDTO = await _walletService.GetWalletSummaryAsync(userId,  page, size);
        return Ok(walletSummaryDTO);
    }
}