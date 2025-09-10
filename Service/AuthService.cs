using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskList_Server.Data;
using TaskList_Server.Interface;
using TaskList_Server.Models.DTOs;
using TaskList_Server.Models;
using Microsoft.EntityFrameworkCore;

namespace TaskList_Server.Service
{
    public class AuthService : IAuthService
    {
        private readonly Tasklist25Context _context;
        private readonly IConfiguration _configuration;

        public AuthService(Tasklist25Context context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<(bool Success, string Message, object? Data)> LoginAsync(LoginRequest req)
        {
            if (string.IsNullOrEmpty(req.UserName) || string.IsNullOrEmpty(req.Password) || string.IsNullOrEmpty(req.CustomerCode))
                return (false, "Username, password, and customer code are required.", null);

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
                return (false, "Invalid credentials or customer code.", null);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName?? ""),
            new Claim("CustomerId", user.IntCustomerId.ToString()??""),
            new Claim("MAccess", user.Maccess.ToString()??"")
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? ""));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["Jwt:ExpiresInHours"])),
                signingCredentials: creds
            );

            var response = new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiresIn = token.ValidTo,
                user
            };

            return (true, "Login successful", response);
        }

        public async Task<(bool Success, string Message)> RegisterAsync(RegisterRequest req)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.UserName == req.UserName || u.Email == req.Email);
            if (existingUser != null)
                return (false, "Username or email already exists");

            using var transaction = await _context.Database.BeginTransactionAsync();
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
                    PassWord = req.Password,
                    Maccess = req.permissionId,
                    IntCustomerId = customer.IntId,
                    BitShowUser = true
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return (true, "Customer and User created successfully");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return (false, ex.Message);
            }
        }

        public async Task<IEnumerable<permissionDto>> GetAllPermissionsAsync()
        {
            return await _context.TblPermissions
                .Select(p => new permissionDto
                {
                    permissionId = p.IntId,
                    ChrPermission = p.ChrPermission ?? ""
                })
                .ToListAsync();
        }


        public async Task<IEnumerable<User>> getAllUsers()
        {
            return await _context.Users
                .Select(p => new User
                {
                    UserId = p.UserId,
                    UserName = p.UserName,
                    Email = p.Email,
                    IntCustomerId = p.IntCustomerId,
                    BitShowUser = p.BitShowUser,
                    Maccess = p.Maccess,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    PassWord = p.PassWord
                })
                .ToListAsync();
        }


    }
}
