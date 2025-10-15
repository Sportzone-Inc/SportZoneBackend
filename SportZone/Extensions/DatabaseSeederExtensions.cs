using SportZone.Services;

namespace SportZone.Extensions
{
    /// <summary>
    /// Extension methods voor database seeding
    /// </summary>
    public static class DatabaseSeederExtensions
    {
        /// <summary>
        /// Seed de database met initiële data
        /// </summary>
        public static async Task SeedDatabaseAsync(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var services = scope.ServiceProvider;
            
            try
            {
                var seeder = services.GetRequiredService<DatabaseSeeder>();
                await seeder.SeedAsync();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }
    }
}
