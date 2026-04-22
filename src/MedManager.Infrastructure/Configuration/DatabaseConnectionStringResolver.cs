using Microsoft.Extensions.Configuration;

using MySqlConnector;

namespace MedManager.Infrastructure.Configuration
{
    public static class DatabaseConnectionStringResolver
    {
        public static string Resolve(IConfiguration? configuration = null)
        {
            var connectionString = GetValue(configuration, "CONNECTION_STRING");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Database configuration missing. Set CONNECTION_STRING.");
            }

            return connectionString;
        }

        public static string DescribeTarget(string connectionString)
        {
            try
            {
                var builder = new MySqlConnectionStringBuilder(connectionString);
                return $"{builder.Server}:{builder.Port}/{builder.Database} (user={builder.UserID})";
            }
            catch
            {
                return "<unparsed-connection-string>";
            }
        }

        private static string? GetValue(IConfiguration? configuration, string key)
        {
            return Environment.GetEnvironmentVariable(key)
                ?? configuration?[key];
        }
    }
}