using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskList_Server.Data;
using TaskList_Server.Models.DTOs;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskList_Server.Models;

namespace TaskList_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly Tasklist25Context _context;
        private readonly IConfiguration _configuration;


        public AuthController(Tasklist25Context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            if (string.IsNullOrEmpty(req.UserName) || string.IsNullOrEmpty(req.Password) || string.IsNullOrEmpty(req.CustomerCode))
                return BadRequest("Username, password, and customer code are required.");

            var user = await _context.Users
                .Where(u => u.UserName == req.UserName
                            && u.PassWord == req.Password
                            && u.IntCustomerId == _context.TblCustomers
                                                      .Where(c => c.ChrCustomerCode == req.CustomerCode)
                                                      .Select(c => c.IntId)
                                                      .FirstOrDefault())
                .Select(u => new
                {
                    u.UserId,
                    u.UserName,
                    u.IntCustomerId,
                    u.Maccess
                })
                .FirstOrDefaultAsync();

            if (user == null)
                return Unauthorized("Invalid credentials or customer code.");

            // Create JWT token
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new Claim("CustomerId", user.IntCustomerId.ToString()),
            new Claim("MAccess", user.Maccess.ToString())
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["Jwt:ExpiresInHours"])),
                signingCredentials: creds
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiresIn = token.ValidTo,
                user
            });
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.UserName == req.UserName || u.Email == req.Email);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Username or email already exists" });
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var customer = new TblCustomer
                    {
                        ChrCustomerCode = req.CustomerCode,
                        ChrCustomerName = "Default Name"
                    };
                    _context.TblCustomers.Add(customer);
                    await _context.SaveChangesAsync();

                    var user = new User
                    {   
                        UserName = req.UserName,
                        Email = req.Email,
                        PassWord = req.Password ,
                        Maccess = req.permissionId,
                        IntCustomerId = customer.IntId,
                        BitShowUser = true
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return Ok(new { message = "Customer and User created successfully" });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return BadRequest(new { message = ex.Message });
                }
            }
        }

        [HttpGet("all")]
        public IActionResult GetAllPermissions()
        {
            try
            {
                var permissions = _context.TblPermissions
                    .Select(p => new permissionDto
                    {
                        permissionId =  p.IntId,
                        ChrPermission = p.ChrPermission 
                    })
                    .ToList();

                return Ok(permissions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
