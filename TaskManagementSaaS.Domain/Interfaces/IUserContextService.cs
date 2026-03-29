using System.Security.Claims;

namespace TaskManagementSaaS.Domain.Interfaces
{
    public interface IUserContextService
    {
        Guid? GetUserId(ClaimsPrincipal? principal = null);
        Guid? GetTenantId(ClaimsPrincipal? principal = null);
        string? GetUserEmail();
        string? GetRole(ClaimsPrincipal? principal = null);
    }
}
