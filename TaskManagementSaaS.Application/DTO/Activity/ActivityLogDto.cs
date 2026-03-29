namespace TaskManagementSaaS.Application.DTO.Activity
{
    public class ActivityLogDto
    {
        public Guid Id { get; set; }
        public string? Message { get; set; }
        public string? UserEmail { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
