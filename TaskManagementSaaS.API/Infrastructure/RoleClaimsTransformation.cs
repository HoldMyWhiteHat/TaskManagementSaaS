using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using TaskManagementSaaS.Domain.Interfaces;

namespace TaskManagementSaaS.API.Infrastructure
{
    public class RoleClaimsTransformation : IClaimsTransformation
    {
        private readonly IUserContextService _userContextService;
        private readonly ILogger<RoleClaimsTransformation> _logger;

        public RoleClaimsTransformation(IUserContextService userContextService, ILogger<RoleClaimsTransformation> logger)
        {
            _userContextService = userContextService;
            _logger = logger;
        }

        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var role = _userContextService.GetRole(principal);
            if (string.IsNullOrEmpty(role))
            {
                var claimsNames = string.Join(", ", principal.Claims.Select(c => $"{c.Type}={c.Value}"));
                _logger.LogWarning("RoleClaimsTransformation: No DB Role found. Principal IsAuthenticated: {IsAuthenticated}, Claims: [{Claims}]", 
                    principal.Identity?.IsAuthenticated, claimsNames);
                return Task.FromResult(principal);
            }

            _logger.LogInformation("RoleClaimsTransformation: Discovered DB Role: {Role} for User", role);

            // Cloned the principal and added a new identity
            var clone = principal.Clone();
            
            // Re-adding the claims as a new ClaimsIdentity ensures they are picked up safely
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(ClaimTypes.Role, role));
            identity.AddClaim(new Claim("role", role)); // for redundancy

            clone.AddIdentity(identity);

            return Task.FromResult(clone);
        }
    }
}
