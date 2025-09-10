using TaskList_Server.Models;
using TaskList_Server.Models.DTOs;

namespace TaskList_Server.Interface
{
    public interface IAuthService
    {
        Task<(bool Success, string Message, object? Data)> LoginAsync(LoginRequest req);
        Task<(bool Success, string Message)> RegisterAsync(RegisterRequest req);
        Task<IEnumerable<permissionDto>> GetAllPermissionsAsync();
        Task<IEnumerable<User>> getAllUsers();
    }
}
