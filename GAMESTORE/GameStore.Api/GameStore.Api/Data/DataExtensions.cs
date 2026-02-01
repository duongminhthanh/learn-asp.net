using Microsoft.EntityFrameworkCore;
namespace GameStore.Api.Data
{
    public static class DataExtensions
    {
        // Extension method to automate database migrations at startup
        public static async Task MigrateDbAsync(this WebApplication app)
        {
            // 1. Create a temporary dependency injection scope
            // This is required because DbContext is a scoped service
            using var scope = app.Services.CreateScope();

            // 2. Resolve the GameStoreContext from the service provider
            // This ensures we have access to the database configuration
            var dbContext = scope.ServiceProvider
                                 .GetRequiredService<GameStoreContext>();

            // 3. Apply any pending migrations to the database asynchronously
            // If the database file (e.g., GameStore.db) doesn't exist, it will be created
            await dbContext.Database.MigrateAsync();
        }
    }
}
