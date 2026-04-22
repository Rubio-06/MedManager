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
            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING")
                ?? config["CONNECTION_STRING"];

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Database connection string not found. Set CONNECTION_STRING in .env.");
            }

            services.AddDbContext<DatabaseContext>(options =>
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString), mySqlOptions =>
                {
                    mySqlOptions.EnableRetryOnFailure(3);
                }));

            return services;
        }
    }
}