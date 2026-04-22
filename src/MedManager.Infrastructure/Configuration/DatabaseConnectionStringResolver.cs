using Microsoft.Extensions.Configuration;

namespace MedManager.Infrastructure.Configuration
{
    public static class DatabaseConnectionStringResolver
    {
        public static string Resolve(IConfiguration? configuration = null)
        {
            var explicitConnectionString = GetValue(configuration, "CONNECTION_STRING")
                ?? GetValue(configuration, "ConnectionStrings:DefaultConnection")
                ?? GetValue(configuration, "ConnectionStrings__DefaultConnection");

            if (!string.IsNullOrWhiteSpace(explicitConnectionString))
            {
                return explicitConnectionString;
            }

            var host = GetValue(configuration, "DB_HOST")
                ?? GetValue(configuration, "MYSQL_HOST")
                ?? "db";

            var port = GetValue(configuration, "DB_PORT")
                ?? GetValue(configuration, "MYSQL_PORT")
                ?? "3306";

            var database = GetValue(configuration, "DB_NAME")
                ?? GetValue(configuration, "MYSQL_DATABASE")
                ?? "medmanager";

            var user = GetValue(configuration, "DB_USER")
                ?? GetValue(configuration, "MYSQL_USER")
                ?? "root";

            var password = GetValue(configuration, "DB_PASSWORD")
                ?? GetValue(configuration, "MYSQL_PASSWORD")
                ?? GetValue(configuration, "MYSQL_ROOT_PASSWORD")
                ?? string.Empty;

            if (string.IsNullOrWhiteSpace(host))
            {
                throw new InvalidOperationException("Database host not found. Set CONNECTION_STRING or DB_HOST in .env.");
            }

            return $"server={host};port={port};database={database};user={user};password={password};";
        }

        private static string? GetValue(IConfiguration? configuration, string key)
        {
            return Environment.GetEnvironmentVariable(key)
                ?? configuration?[key];
        }
    }
}