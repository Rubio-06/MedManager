using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

using MedManager.Infrastructure.Configuration;

namespace MedManager.Infrastructure.Context
{
    public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            DotEnvLoader.Load();

            // Utiliser une version MySQL spécifique au lieu de AutoDetect pour éviter les problèmes de connexion
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 21));

            var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Database connection string not found. Set CONNECTION_STRING in .env.");
            }

            optionsBuilder.UseMySql(connectionString, serverVersion, mySqlOptions =>
            {
                mySqlOptions.EnableRetryOnFailure(3);
            });

            return new DatabaseContext(optionsBuilder.Options);
        }
    }
}
