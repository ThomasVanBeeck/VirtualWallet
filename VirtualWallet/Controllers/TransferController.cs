using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using VirtualWallet.Dtos;
using VirtualWallet.Services;

namespace VirtualWallet.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = "CookieAuth")]
[Route("api/[controller]")]
public class TransferController : ControllerBase
{
    private readonly TransferService _transferService;
    

    public TransferController(TransferService transferService)
    {
        _transferService = transferService;
    }

    [HttpPost]
    public async Task<IActionResult> AddTransfer([FromBody] TransferDto transferDto)
    {
        await _transferService.AddTransferAsync(transferDto);
        return Ok();
    }
}