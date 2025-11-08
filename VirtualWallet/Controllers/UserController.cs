using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using VirtualWallet.Services;

namespace VirtualWallet.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }
        
        [Authorize(AuthenticationSchemes = "CookieAuth")]
        [HttpGet("testuser")]
        public async Task<IActionResult> GetTestUser()
        {
            var user = await _userService.GetCurrentUserAsync("testuser1");
            if (user == null) return NotFound();
            return Ok(user);
        }
        
        [Authorize(AuthenticationSchemes = "CookieAuth")]
        [HttpGet("current-user")]
        public async Task<IActionResult> GetCurrentUser()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // WIP: add query check in db to see if user exists and get other
            // attributes besides the username, like returning firstname for example
            var userDTO = await _userService.GetCurrentUserAsync("testuser1");
            if (userDTO == null) return NotFound();
            
            if (string.IsNullOrEmpty(userId)) return Unauthorized();
            return Ok(userDTO);
        }
    }
}