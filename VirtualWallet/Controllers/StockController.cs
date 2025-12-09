using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtualWallet.Services;

namespace VirtualWallet.Controllers;
using Dtos;

[ApiController]
[Route("api/[controller]")]
public class StockController :  ControllerBase
{
    private readonly StockService _stockService;

    public StockController(StockService stockService)
    {
        _stockService = stockService;
    }

    [Authorize(AuthenticationSchemes = "CookieAuth")]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllStocks()
    {
        var stocks = await _stockService.GetAllStocks();
        return Ok(stocks);
    }
}