using MedManager.Infrastructure.Context;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MedManager.Infrastructure.Services
{
    public static class ContextService
    {
        public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");

            services.AddDbContext<DatabaseContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), mySqlOptions =>
                {
                    mySqlOptions.EnableRetryOnFailure(3);
                }));

            return services;
        }
    }
}