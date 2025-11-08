using Microsoft.AspNetCore.Mvc;
using VirtualWallet.Services;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace VirtualWallet.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController: ControllerBase
{
    private readonly UserService _userService;

    public AuthController(UserService userService)
    {
        _userService = userService;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] UserLoginDTO dto)
    {
        var user = await _userService.LoginAsync(dto.Username, dto.Password);
            
        if (user == null)
        {
            return Unauthorized();
        }
            
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, "Regular user") 
        };

        var claimsIdentity = new ClaimsIdentity(claims, "CookieAuth"); 
        // tell browser to use cookie
        await HttpContext.SignInAsync("CookieAuth", new ClaimsPrincipal(claimsIdentity));
        return Ok(new { Message = "Logged in successfully." });
    }
        
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        // tell browser to remove cookie
        await HttpContext.SignOutAsync("CookieAuth");
            
        return Ok(new { Message = "Logged out successfully." });
    }
}