using System.ComponentModel.DataAnnotations;

namespace TaskManagementSaaS.Application.DTO.Tenants
{
    public class TenantDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CreateTenantDto
    {
        [Required(ErrorMessage = "Workspace name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Workspace name must be between 2 and 100 characters.")]
        public string Name { get; set; } = string.Empty;
    }

    public class UpdateTenantDto
    {
        [Required(ErrorMessage = "Workspace name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Workspace name must be between 2 and 100 characters.")]
        public string Name { get; set; } = string.Empty;
    }
}
