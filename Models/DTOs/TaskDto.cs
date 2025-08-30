namespace TaskList_Server.Models.DTOs
{
    public class TaskDto
    {
        public int TaskId { get; set; }
        public int IntDisplayNo { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime? LastChangeDate { get; set; }
        public string DelegatedTo { get; set; }
        public string Description { get; set; }
        public bool Visible { get; set; }
        public bool SeriousBug { get; set; }
        public bool SmallBug { get; set; }

        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }

        public int StatusId { get; set; }
        public string StatusName { get; set; }

        public int PriorityId { get; set; }
        public string PriorityName { get; set; }

        public string ApplicationName { get; set; }
        public int AppId { get; set; }
    }
}
