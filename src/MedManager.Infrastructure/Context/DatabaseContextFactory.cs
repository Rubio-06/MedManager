using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace MedManager.Infrastructure.Context
{
    public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();

            // Utiliser une version MySQL spécifique au lieu de AutoDetect pour éviter les problèmes de connexion
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 21));

            var connectionString = "server=localhost;database=medmanager;user=root;password=;";

            optionsBuilder.UseMySql(connectionString, serverVersion, mySqlOptions =>
            {
                mySqlOptions.EnableRetryOnFailure(3);
            });

            return new DatabaseContext(optionsBuilder.Options);
        }
    }
}
