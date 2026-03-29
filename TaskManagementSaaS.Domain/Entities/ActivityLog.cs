namespace TaskManagementSaaS.Domain.Entities
{
    public class ActivityLog
    {
        public Guid Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? UserEmail { get; set; }
        public Guid TenantId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Navigation
        public Tenant? Tenant { get; set; }
    }
}
