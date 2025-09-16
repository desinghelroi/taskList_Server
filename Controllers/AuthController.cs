using Microsoft.AspNetCore.Mvc;
using TaskList_Server.Models.DTOs;
using TaskList_Server.Interface;
using TaskList_Server.Models;
using TaskList_Server.Data;

namespace TaskList_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService, Tasklist25Context context) : ControllerBase
    {
        private readonly IAuthService _authService = authService;
        private readonly Tasklist25Context _context = context;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var result = await _authService.LoginAsync(req);
            if (!result.Success)
                return Unauthorized(new { message = result.Message });

            return Ok(result.Data);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req) => await _authService.RegisterAsync(req) is var result && result.Success ? Ok(new { message = result.Message }) : BadRequest(new { message = result.Message });

        [HttpGet("all")]
        public async Task<IActionResult> GetAllPermissions() => Ok(await _authService.GetAllPermissionsAsync());

        [HttpGet("get_AllUsers")]
        public async Task<IActionResult> GetAllUsers() => Ok(await _authService.getAllUsers());

        [HttpGet("get_userById/{id}")]
        public async Task<ActionResult<User>> GetUserById(int id) => await _context.Users.FindAsync(id) is var user && user is not null ? Ok(user) : NotFound();
        [HttpDelete("deleteusers/{id}")]
        public async Task<ActionResult> DeleteUser(int id) => await _context.Users.FindAsync(id) is var user && user is not null ? (_context.Users.Remove(user), await _context.SaveChangesAsync(), NoContent()).Item3 : NotFound();


        [HttpPut("updateusers/{id}")]
        public async Task<ActionResult<User>> UpdateUser(int id, [FromBody] User user)
        {
            if (id != user.UserId)
                return BadRequest("User ID mismatch");

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null)
                return NotFound();

            existingUser.UserName = user.UserName;
            existingUser.PassWord = user.PassWord;
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.Maccess = user.Maccess;
            existingUser.Email = user.Email;
            existingUser.IntCustomerId = user.IntCustomerId;
            existingUser.BitShowUser = user.BitShowUser;

            await _context.SaveChangesAsync();

            return Ok(existingUser);
        }

        [HttpPost("createusers")]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user)
        {
            if (user == null)
                return BadRequest();

            User createUser = new User();
            createUser.UserName = user.UserName;
            createUser.PassWord = user.PassWord;
            createUser.FirstName = user.FirstName;
            createUser.LastName = user.LastName;
            createUser.Maccess = user.Maccess;
            createUser.Email = user.Email;
            createUser.IntCustomerId = 1;
            createUser.BitShowUser = user.BitShowUser;
            _context.Users.Add(createUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUserById), new { id = user.UserId }, user);
        }
    }
}
