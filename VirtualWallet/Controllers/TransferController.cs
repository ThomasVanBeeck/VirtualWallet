using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using VirtualWallet.DTOs;
using VirtualWallet.Services;

namespace VirtualWallet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransferController : ControllerBase
{
    private readonly TransferService _transferService;
    private readonly UserService _userService;
    

    public TransferController(TransferService transferService, UserService userService)
    {
        _transferService = transferService;
        _userService = userService;
    }

    [Authorize(AuthenticationSchemes = "CookieAuth")]
    [HttpPost]
    public async Task<IActionResult> AddTransfer([FromBody] TransferDTO transferDTO)
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