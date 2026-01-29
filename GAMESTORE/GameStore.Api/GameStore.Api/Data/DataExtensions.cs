using Microsoft.EntityFrameworkCore;
namespace GameStore.Api.Data
{
    public static class DataExtensions
    {
        // Extension method to automatically apply pending migrations to the database
        public static void MigrateDb(this WebApplication app)
        {
            // Create a temporary scope to resolve scoped services like DbContext
            using var scope = app.Services.CreateScope();

            // Retrieve the DbContext instance from the service provider
            // Note: Ensure GameStoreContext is registered in Program.cs
            var dbContext = scope.ServiceProvider
                .GetRequiredService<GameStoreContext>();

            // Apply any pending migrations to the database
            // This will create the database file (e.g., GameStore.db) if it doesn't exist
            dbContext.Database.Migrate();
        }
    }
}
