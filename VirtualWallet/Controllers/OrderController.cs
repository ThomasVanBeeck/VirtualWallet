using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtualWallet.Dtos;
using VirtualWallet.Services;

namespace VirtualWallet.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = "CookieAuth")]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly OrderService _orderService;
    
    public OrderController(OrderService orderService)
    {
        _orderService = orderService;
    }
    
    [HttpPost]
    public async Task<IActionResult> AddOrder([FromBody] OrderPostDto orderPostDto)
    {
        await _orderService.AddOrderAsync(orderPostDto);
        return Ok();
    }
    
    [HttpGet]
    public async Task<IActionResult> GetOrders(
        [FromQuery] int page = 2, [FromQuery] int size = 1)
    {
        var ordersPaginatedDto = await _orderService.GetOrdersAsync(page, size);
        return Ok(ordersPaginatedDto);
    }
}