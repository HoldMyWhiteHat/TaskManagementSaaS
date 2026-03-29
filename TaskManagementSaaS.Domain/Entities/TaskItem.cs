using TaskManagementSaaS.Domain.Enums;

namespace TaskManagementSaaS.Domain.Entities
{
    public class TaskItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public TaskItemStatus Status { get; set; } = TaskItemStatus.Open;
        
        public Guid ProjectId { get; set; }
        public Guid TenantId { get; set; }
        public Guid? AssignedUserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public Project? Project { get; set; }
        public User? AssignedUser { get; set; }
    }
}