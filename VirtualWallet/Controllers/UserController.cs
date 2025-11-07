using Microsoft.AspNetCore.Mvc;
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

        [HttpGet("currentuser")]
        public async Task<IActionResult> GetCurrentUser([FromBody]  UserDto dto)
        {
            var user = await _userService.GetCurrentUserAsync(dto.Username);
            if (user == null) return NotFound();
            return Ok(user);
        }
        
        [HttpGet("testuser")]
        public async Task<IActionResult> GetTestUser()
        {
            var user = await _userService.GetCurrentUserAsync("testuser1");
            if (user == null) return NotFound();
            return Ok(user);
        }
        
        
        /*
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            var user = await _userService.LoginAsync(dto.Username, dto.Password);
            if (user == null) return Unauthorized();
        
            // hier kun je eventueel een session-cookie zetten
            HttpContext.Response.Cookies.Append("sessionId", "dummy-session"); 

            return Ok(new { user.Id, user.Username, user.Email });
        }
        */
        
        
    }
}