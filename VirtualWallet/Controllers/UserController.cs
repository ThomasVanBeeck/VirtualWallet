using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using VirtualWallet.Dtos;
using VirtualWallet.Services;

namespace VirtualWallet.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = "CookieAuth")]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }
    
    [HttpGet("current-user")]
    public async Task<ActionResult<UserDto>> GetCurrentUser() 
    {
        var userDto = await _userService.GetCurrentUserAsync();

        if (userDto == null) 
            return NotFound("User not found.");

        return Ok(userDto);
    }

    [AllowAnonymous]
    [HttpGet("exists/{username}")]
    public async Task<ActionResult<bool>> GetExists(string username)
    {
        bool isKnownUsername = await _userService.GetUsernameExists(username);
        return Ok(isKnownUsername);
    }

    [AllowAnonymous]
    [HttpPost("create-user")]
    public async Task<IActionResult> CreateUser([FromBody] UserRegisterDto userRegisterDto)
    {
        try
        {
            await _userService.CreateUserAsync(userRegisterDto);
            return Ok();
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new { message = exception.Message });
        }
    }
}
