using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtualWallet.DTOs;
using VirtualWallet.Services;

namespace VirtualWallet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WalletController : ControllerBase
{
    private readonly WalletService _walletService;
    private readonly UserService _userService;

    public WalletController(WalletService walletService, UserService userService)
    {
        _walletService = walletService;
        _userService = userService;
    }

    [Authorize(AuthenticationSchemes = "CookieAuth")]
    [HttpGet]
    public async Task<ActionResult<WalletSummaryDTO>> GetWalletSummary(
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