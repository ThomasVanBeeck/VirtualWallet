using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtualWallet.Services;

namespace VirtualWallet.Controllers;
using Dtos;

[ApiController]
[Authorize(AuthenticationSchemes = "CookieAuth")]
[Route("api/[controller]")]
public class StockController :  ControllerBase
{
    private readonly StockService _stockService;

    public StockController(StockService stockService)
    {
        _stockService = stockService;
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllStocks()
    {
        var stocks = await _stockService.GetAllStocks();
        return Ok(stocks);
    }
    
    [HttpGet("lastupdate")]
    public async Task<IActionResult> GetLastUpdateTimestamp()
    {
        var stockUpdateDto = await _stockService.GetLastUpdateTimestamp();
        return Ok(stockUpdateDto);
    }
}