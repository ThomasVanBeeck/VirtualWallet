using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using VirtualWallet.Dtos;
using VirtualWallet.Services;

namespace VirtualWallet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransferController : ControllerBase
{
    private readonly TransferService _transferService;
    

    public TransferController(TransferService transferService)
    {
        _transferService = transferService;
    }

    [Authorize(AuthenticationSchemes = "CookieAuth")]
    [HttpPost]
    public async Task<IActionResult> AddTransfer([FromBody] TransferDto transferDTO)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdString))
            return Unauthorized();
        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized(); 
        
        await _transferService.AddTransferAsync(userId, transferDTO);
        return Ok();
    }
}