using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using TaskManagementSaaS.Domain.Interfaces;

namespace TaskManagementSaaS.Infrastructure.Services
{
    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _cache;

        public UserContextService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IMemoryCache cache)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
            _cache = cache;
        }

        private (Guid UserId, Guid TenantId, string Role)? ResolveUser(ClaimsPrincipal? principal = null)
        {
            var user = principal ?? _httpContextAccessor.HttpContext?.User;
            var subjectId = user?.FindFirstValue(ClaimTypes.NameIdentifier) ?? user?.FindFirstValue("sub");
            if (string.IsNullOrEmpty(subjectId))
            {
                // Only log if we are actually in a web request
                if (_httpContextAccessor.HttpContext != null)
                {
                    Console.WriteLine("[UserContext] ResolveUser failed: Could not find NameIdentifier or sub in User Claims.");
                }
                return null;
            }

            if (_cache.TryGetValue(subjectId, out (Guid UserId, Guid TenantId, string Role) cached))
            {
                return cached;
            }

            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                if (string.IsNullOrEmpty(connectionString)) return null;

                using var connection = new SqlConnection(connectionString);
                connection.Open();

                using var command = new SqlCommand(
                    "SELECT TOP 1 Id, TenantId, Role FROM Users WHERE ExternalSubjectId = @SubjectId",
                    connection);
                command.Parameters.AddWithValue("@SubjectId", subjectId);

                using var reader = command.ExecuteReader();
                if (reader.Read())
                {
                    var userId = reader.GetGuid(0);
                    var tenantId = reader.GetGuid(1);
                    var role = reader.GetString(2);

                    var result = (userId, tenantId, role);
                    _cache.Set(subjectId, result, TimeSpan.FromMinutes(5));
                    
                    Console.WriteLine($"[UserContext] Success: Resolved user {subjectId} with Role {role}");
                    return result;
                }
                else
                {
                    Console.WriteLine($"[UserContext] User not found in DB for SubjectId: {subjectId}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UserContext] Error during DB fetch: {ex.Message}");
            }

            return null;
        }

        public Guid? GetTenantId(ClaimsPrincipal? principal = null) => ResolveUser(principal)?.TenantId;
        public Guid? GetUserId(ClaimsPrincipal? principal = null) => ResolveUser(principal)?.UserId;
        public string? GetRole(ClaimsPrincipal? principal = null) => ResolveUser(principal)?.Role;

        public string? GetUserEmail()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email)
                ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("email");
        }
    }
}
