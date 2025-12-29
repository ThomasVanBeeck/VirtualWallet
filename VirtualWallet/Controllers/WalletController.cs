using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtualWallet.Dtos;
using VirtualWallet.Services;

namespace VirtualWallet.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = "CookieAuth")]
[Route("api/[controller]")]
public class WalletController : ControllerBase
{
    private readonly WalletService _walletService;

    public WalletController(WalletService walletService)
    {
        _walletService = walletService;
    }
    
    [HttpGet]
    public async Task<ActionResult<WalletSummaryDto>> GetWalletSummary(
        [FromQuery] int page = 1, [FromQuery] int size = 1)
    {
        var walletSummaryDto = await _walletService.GetWalletSummaryAsync(page, size);
        return Ok(walletSummaryDto);
    }
}