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

}
