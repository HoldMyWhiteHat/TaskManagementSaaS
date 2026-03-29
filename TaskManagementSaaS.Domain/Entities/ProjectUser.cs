namespace TaskManagementSaaS.Domain.Entities
{
    public class ProjectUser
    {
        public Guid ProjectId { get; set; }
        public Guid UserId { get; set; }
        public Guid TenantId { get; set; }

        // Navigation
        public Project? Project { get; set; }
        public User? User { get; set; }
    }
}
