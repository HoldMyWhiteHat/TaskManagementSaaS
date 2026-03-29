using System.ComponentModel.DataAnnotations;
using TaskManagementSaaS.Domain.Enums;

namespace TaskManagementSaaS.Application.DTO.Tasks
{
    public class TaskDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TaskPriority Priority { get; set; }
        public TaskItemStatus Status { get; set; }
        public Guid ProjectId { get; set; }
        public string? AssignedUserEmail { get; set; }
        public string? AssigneeName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateTaskDto
    {
        [Required(ErrorMessage = "Task title is required.")]
        [StringLength(300, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 300 characters.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [EnumDataType(typeof(TaskPriority), ErrorMessage = "Invalid priority value.")]
        public TaskPriority Priority { get; set; }

        [Required(ErrorMessage = "Project is required.")]
        public Guid ProjectId { get; set; }
    }

    public class UpdateTaskDto
    {
        [Required(ErrorMessage = "Task title is required.")]
        [StringLength(300, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 300 characters.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [EnumDataType(typeof(TaskPriority), ErrorMessage = "Invalid priority value.")]
        public TaskPriority Priority { get; set; }

        [Required]
        [EnumDataType(typeof(TaskItemStatus), ErrorMessage = "Invalid status value.")]
        public TaskItemStatus Status { get; set; }
    }
}
