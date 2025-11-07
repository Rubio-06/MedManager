namespace MedManager.Infrastructure.Database
{
    public static class DatabaseExtensions
    {
        public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider, string environment)
        {
            await DbInitializer.Initialize(serviceProvider, environment);
        }
    }
}
