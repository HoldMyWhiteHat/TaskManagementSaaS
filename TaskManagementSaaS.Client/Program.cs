using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using TaskManagementSaaS.Client;
using TaskManagementSaaS.Client.Infrastructure;
using TaskManagementSaaS.Client.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Hardcoded Auth0 Configuration for reliability during deployment prep
// Authority and ClientId are public values for SPAs.
const string authority = "https://dev-5rjxr6k61ht2n50r.us.auth0.com/";
const string clientId = "oQAnzCrYGIhyKF60sS54p2t510Y0dGBj";
const string audience = "TaskManagementSaaS.API";

builder.Services.AddOidcAuthentication(options =>
{
    options.ProviderOptions.Authority = authority;
    options.ProviderOptions.ClientId = clientId;
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.DefaultScopes.Add("openid");
    options.ProviderOptions.DefaultScopes.Add("profile");
    options.ProviderOptions.DefaultScopes.Add("email");
    options.ProviderOptions.AdditionalProviderParameters.Add("audience", audience);
    options.UserOptions.RoleClaim = "https://taskmanagement.com/roles";
}).AddAccountClaimsPrincipalFactory<CustomUserFactory>();

builder.Services.AddScoped<CustomAuthorizationMessageHandler>();

builder.Services.AddHttpClient("TaskManagementSaaS.API", client => 
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient("TaskManagementSaaS.API"));

// Register Services
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<TaskService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ActivityService>();

await builder.Build().RunAsync();
