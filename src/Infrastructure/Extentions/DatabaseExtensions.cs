using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace Infrastructure.Extentions
{
    public static class DatabaseExtensions
    {
        public static async Task ApplyMigrationsAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();

            try
            {
                logger.LogInformation("Starting PostgreSQL database initialization...");

                var context = services.GetRequiredService<ApplicationDbContext>();

                // Esperar hasta que PostgreSQL esté disponible
                await WaitForDatabaseAsync(context, logger);

                // Aplicar migraciones
                logger.LogInformation("Applying database migrations...");
                await context.Database.MigrateAsync();

                logger.LogInformation("PostgreSQL database initialization completed successfully!");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing PostgreSQL database");
                throw;
            }
        }

        private static async Task WaitForDatabaseAsync(ApplicationDbContext context, ILogger logger)
        {
            var maxRetries = 20;
            var delay = TimeSpan.FromSeconds(5);

            for (int i = 1; i <= maxRetries; i++)
            {
                try
                {
                    logger.LogInformation("Attempting to connect to PostgreSQL (attempt {Attempt}/{MaxRetries})...", i, maxRetries);

                    await context.Database.CanConnectAsync();

                    logger.LogInformation("Successfully connected to PostgreSQL!");
                    return;
                }
                catch (Exception ex) when (ex is NpgsqlException || ex is InvalidOperationException)
                {
                    if (i == maxRetries)
                    {
                        logger.LogError("Failed to connect to PostgreSQL after {MaxRetries} attempts", maxRetries);
                        throw;
                    }

                    logger.LogWarning("Connection attempt {Attempt} failed: {Error}. Retrying in {Delay} seconds...",
                        i, ex.Message, delay.TotalSeconds);

                    await Task.Delay(delay);
                }
            }
        }
    }
}
