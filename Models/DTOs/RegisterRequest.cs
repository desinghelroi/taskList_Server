namespace TaskList_Server.Models.DTOs
{
    public class RegisterRequest
    {
        public string CustomerCode { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int permissionId { get; set; }
    }
}
