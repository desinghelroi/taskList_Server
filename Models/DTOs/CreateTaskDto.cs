namespace TaskList_Server.Models.DTOs
{
    public class CreateTaskDto
    {
        public int AppId { get; set; }
        public int UserId { get; set; }
        public int StatusId { get; set; }
        public int PriorityId { get; set; }
        public bool SeriousBug { get; set; }
        public bool SmallBug { get; set; }
        public bool Visible { get; set; }
        public string Description { get; set; }
        public IFormFile? File { get; set; }
    }
}
