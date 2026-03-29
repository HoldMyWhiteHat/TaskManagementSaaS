namespace TaskManagementSaaS.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string? ExternalSubjectId { get; set; } // Auth0 Sub
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = "User"; // Admin, Manager, User
        public Guid TenantId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Tenant? Tenant { get; set; }
        public ICollection<ProjectUser> ProjectUsers { get; set; } = new List<ProjectUser>();
    }
}
