namespace TaskList_Server.Models.DTOs
{
    public class StatusDto
    {
        public int StatusId { get; set; }
        public string Name { get; set; }
    }

    public class ProjectsDto
    {
        public int AppId { get; set; }
        public string ApplicationName { get; set; }
    }

    public class PriorityDto
    {
        public int PriorityId { get; set; }
        public string PriorityName { get; set; }
    }
    public class DeveloperDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public bool IsActive { get; set; }
    }

    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string CustomerCode { get; set; }
    }
    public class permissionDto
    {
        public int permissionId { get; set; }
        public string ChrPermission { get; set; }
    }

    public class ReportFilters
    {
        public int? Status { get; set; }
        public string? TaskName { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int? ProjectId { get; set; }
        public int? DeveloperId { get; set; }
    }


    public class TasksReportDto
    { 
        public int Id { get; set; }
        public string DeveloperName { get; set; }
        public string TaskName { get; set; }
        public string StatusName { get; set; }
        public string ProjectName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }


}
