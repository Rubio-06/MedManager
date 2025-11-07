using MedManager.Infrastructure.Context;
using MedManager.Infrastructure.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MedManager.Infrastructure
{
    public static class Infrastructure
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Add Database Context
            services.AddDatabaseServices(configuration);

            // Add Identity Services
            services.AddIdentityServices();

            // Add Cookie Services
            services.AddCookieServices();

            return services;
        }
    }
}
