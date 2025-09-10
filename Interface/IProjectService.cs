using TaskList_Server.Models;
using TaskList_Server.Models.DTOs;

namespace TaskList_Server.Interface
{
    public interface IProjectService
    {
        Task<IEnumerable<TblApplication>> GetAllProjectsAsync();
        Task<TblApplication> CreateProjectAsync(TblApplication app, string? customerId);
        Task<TblApplication?> UpdateProjectAsync(int id, TblApplication app, string? customerId);
        Task<bool> DeleteProjectAsync(int id);
        Task<IEnumerable<EmployeeTaskStatsDto>> GetEmployeeTaskStatsAsync(DateTime fromDate, DateTime toDate);
    }
}
