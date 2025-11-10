using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using VirtualWallet.DTOs;
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
        [HttpGet("current-user")]
        public async Task<ActionResult<UserDTO?>> GetCurrentUser() 
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userIdString))
                return Unauthorized(); 
            if (!Guid.TryParse(userIdString, out var id))
                return Unauthorized(); 
            
            var userDTO = await _userService.GetCurrentUserAsync(id);

            if (userDTO == null) 
                return NotFound("User not found based on cookie ID.");

            return Ok(userDTO);
        }

        [HttpGet("exists/{username}")]
        public async Task<ActionResult<bool>> GetExists(string username)
        {
            bool isKnownUsername = await _userService.GetUsernameExists(username);
            return Ok(isKnownUsername);
        }
    }
}