using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using System.Security.Claims;
using TaskManagementSaaS.Client.Services;

namespace TaskManagementSaaS.Client.Infrastructure
{
    public class CustomUserFactory : AccountClaimsPrincipalFactory<RemoteUserAccount>
    {
        private readonly ILogger<CustomUserFactory> _logger;
        private readonly IServiceProvider _serviceProvider;

        // Static flag to communicate redirect need to components
        public static bool RequiresTenantCreation { get; set; }

        public CustomUserFactory(IAccessTokenProviderAccessor accessor, ILogger<CustomUserFactory> logger, IServiceProvider serviceProvider) 
            : base(accessor)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public override async ValueTask<ClaimsPrincipal> CreateUserAsync(
            RemoteUserAccount account, 
            RemoteAuthenticationUserOptions options)
        {
            var user = await base.CreateUserAsync(account, options);

            if (user.Identity?.IsAuthenticated == true)
            {
                var identity = (ClaimsIdentity)user.Identity;
                
                try 
                {
                    using var scope = _serviceProvider.CreateScope();
                    var userService = scope.ServiceProvider.GetRequiredService<UserService>();

                    var name = user.FindFirst("name")?.Value ?? user.FindFirst(ClaimTypes.Name)?.Value;
                    var email = user.FindFirst("email")?.Value ?? user.FindFirst(ClaimTypes.Email)?.Value;

                    var syncResult = await userService.SyncUserAsync(email, name);

                    if (syncResult != null)
                    {
                        if (syncResult.RequiresTenantName)
                        {
                            // Signal that this user needs to create a workspace
                            RequiresTenantCreation = true;
                            _logger.LogInformation("New user requires tenant creation, will redirect.");
                        }
                        else
                        {
                            RequiresTenantCreation = false;

                            if (!string.IsNullOrEmpty(syncResult.Role))
                            {
                                identity.AddClaim(new Claim(ClaimTypes.Role, syncResult.Role));
                                identity.AddClaim(new Claim("https://taskmanagement.com/roles", syncResult.Role));
                            }
                            if (syncResult.TenantId.HasValue)
                            {
                                identity.AddClaim(new Claim("TenantId", syncResult.TenantId.Value.ToString()));
                            }
                            if (!string.IsNullOrEmpty(syncResult.TenantName))
                            {
                                identity.AddClaim(new Claim("TenantName", syncResult.TenantName));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error syncing user in CustomUserFactory");
                }
            }

            return user;
        }
    }
}
