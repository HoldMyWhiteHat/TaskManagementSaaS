using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;

namespace TaskManagementSaaS.Client.Infrastructure
{
    public class CustomAuthorizationMessageHandler : AuthorizationMessageHandler
    {
        public CustomAuthorizationMessageHandler(IAccessTokenProvider accessor,
            NavigationManager navigationManager)
            : base(accessor, navigationManager)
        {
            ConfigureHandler(
                authorizedUrls: new[] { navigationManager.BaseUri },
                scopes: new[] { "openid", "profile", "email" });
        }
    }
}
