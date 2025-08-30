namespace TaskList_Server.Models.DTOs
{
    public class CreateTaskDto
    {
        public string Project { get; set; }
        public string AssignedTo { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public bool SeriousBug { get; set; }
        public bool MinorBug { get; set; }
        public string Visible { get; set; }
        public string Description { get; set; }
        public IFormFile? File { get; set; }  // optional upload
    }
}
