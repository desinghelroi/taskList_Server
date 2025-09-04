using Microsoft.AspNetCore.Mvc;
using TaskList_Server.Models.DTOs;
using TaskList_Server.Interface;

namespace TaskList_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var result = await _authService.LoginAsync(req);
            if (!result.Success)
                return Unauthorized(new { message = result.Message });

            return Ok(result.Data);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            var result = await _authService.RegisterAsync(req);
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new { message = result.Message });
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllPermissions()
        {
            var permissions = await _authService.GetAllPermissionsAsync();
            return Ok(permissions);
        }
    }
}
