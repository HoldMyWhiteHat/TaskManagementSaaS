using Microsoft.Extensions.DependencyInjection;
using TaskManagementSaaS.Application.Interfaces;
using TaskManagementSaaS.Domain.Interfaces;
using TaskManagementSaaS.Infrastructure.Services;

namespace TaskManagementSaaS.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<IUserContextService, UserContextService>();
            services.AddScoped<IBlobStorageService, AzureBlobStorageService>();
            
            return services;
        }
    }
}
