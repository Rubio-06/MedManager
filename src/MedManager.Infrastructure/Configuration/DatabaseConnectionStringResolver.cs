using Microsoft.Extensions.Configuration;

using MySqlConnector;

namespace MedManager.Infrastructure.Configuration
{
    public static class DatabaseConnectionStringResolver
    {
        public static string Resolve(IConfiguration? configuration = null)
        {
            var explicitConnectionString = GetValue(configuration, "CONNECTION_STRING")
                ?? GetValue(configuration, "DATABASE_URL")
                ?? GetValue(configuration, "MYSQL_URL")
                ?? GetValue(configuration, "JAWSDB_URL")
                ?? GetValue(configuration, "ConnectionStrings:DefaultConnection")
                ?? GetValue(configuration, "ConnectionStrings__DefaultConnection");

            if (!string.IsNullOrWhiteSpace(explicitConnectionString))
            {
                return NormalizeConnectionString(explicitConnectionString);
            }

            var host = GetValue(configuration, "DB_HOST")
                ?? GetValue(configuration, "MYSQL_HOST");

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
                throw new InvalidOperationException(
                    "Database configuration missing. Set CONNECTION_STRING (or DATABASE_URL) or DB_HOST/DB_PORT/DB_NAME/DB_USER/DB_PASSWORD.");
            }

            return $"server={host};port={port};database={database};user={user};password={password};";
        }

        public static string DescribeTarget(string connectionString)
        {
            try
            {
                var builder = new MySqlConnectionStringBuilder(NormalizeConnectionString(connectionString));
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

        private static string NormalizeConnectionString(string rawValue)
        {
            if (!Uri.TryCreate(rawValue, UriKind.Absolute, out var uri))
            {
                return rawValue;
            }

            if (!string.Equals(uri.Scheme, "mysql", StringComparison.OrdinalIgnoreCase))
            {
                return rawValue;
            }

            var userInfo = uri.UserInfo.Split(':', 2);
            var user = userInfo.Length > 0 ? Uri.UnescapeDataString(userInfo[0]) : "root";
            var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty;
            var database = uri.AbsolutePath.Trim('/');

            var builder = new MySqlConnectionStringBuilder
            {
                Server = uri.Host,
                Port = (uint)(uri.Port > 0 ? uri.Port : 3306),
                Database = string.IsNullOrWhiteSpace(database) ? "medmanager" : database,
                UserID = user,
                Password = password
            };

            return builder.ConnectionString;
        }
    }
}