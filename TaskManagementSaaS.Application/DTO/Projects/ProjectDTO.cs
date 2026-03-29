using System.ComponentModel.DataAnnotations;

namespace TaskManagementSaaS.Application.DTO.Projects
{
    public class ProjectDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int TaskCount { get; set; }
        public int MemberCount { get; set; }
        public List<ProjectMemberDto> Members { get; set; } = new();
    }

    public class CreateProjectDto
    {
        [Required(ErrorMessage = "Project name is required.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Project name must be between 2 and 200 characters.")]
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateProjectDto
    {
        [Required(ErrorMessage = "Project name is required.")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Project name must be between 2 and 200 characters.")]
        public string Name { get; set; } = string.Empty;
    }
}
