using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VirtualWallet.DTOs;
using VirtualWallet.Services;

namespace VirtualWallet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly OrderService _orderService;


    public OrderController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [Authorize(AuthenticationSchemes = "CookieAuth")]
    [HttpPost]
    public async Task<IActionResult> AddOrder([FromBody] OrderPostDTO orderPostDTO)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdString))
            return Unauthorized();
        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized();
        
        await _orderService.AddOrderAsync(userId, orderPostDTO);
        return Ok();
    }

    [Authorize(AuthenticationSchemes = "CookieAuth")]
    [HttpGet]
    public async Task<IActionResult> GetOrders(
        [FromQuery] int page = 2, [FromQuery] int size = 1)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        if (string.IsNullOrEmpty(userIdString))
            return Unauthorized();
        if (!Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        var ordersPaginatedDTO = await _orderService.GetOrdersAsync(userId, page, size);
        return Ok(ordersPaginatedDTO);
    }
}