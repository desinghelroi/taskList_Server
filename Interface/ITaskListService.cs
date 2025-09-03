using TaskList_Server.Models.DTOs;

namespace TaskList_Server.Interface
{
    public interface ITaskListService
    {
        Task<PagedResult<TaskDto>> GetTasksAsync(string filter, string search, string status, int page, int pageSize, string customerId);
        Task<TaskDto?> GetTaskByIdAsync(int id);
        Task<(bool Success, string Message, int? TaskId)> CreateTaskAsync(CreateTaskDto dto, int customerId);
        Task<(bool Success, string Message)> UpdateTaskAsync(int id, TaskDto dto);
        Task<(bool Success, string Message)> DeleteTaskAsync(int id);
        Task<IEnumerable<StatusDto>> GetStatusesAsync();
        Task<IEnumerable<PriorityDto>> GetPriorityListAsync();
        Task<IEnumerable<DeveloperDto>> GetDevelopersAsync();
        Task<IEnumerable<ProjectsDto>> GetProjectListAsync();
        Task<TaskCountsDto> GetCountsAsync(string customerId);
        Task<IEnumerable<TasksReportDto>> GetTasksReportAsync(ReportFilters filters);
        Task<TaskFileDto?> GetFileContentAsync(int id);
    }
}
