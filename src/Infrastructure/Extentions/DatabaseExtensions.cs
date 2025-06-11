using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Extentions
{
    public static class DatabaseExtensions
    {
        public static async Task ApplyMigrationsAsync(this IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<ApplicationDbContext>>();
            var configuration = services.GetRequiredService<IConfiguration>();

            try
            {
                logger.LogInformation("Starting database initialization...");

                // ✅ Paso 1: Crear la base de datos si no existe
                await EnsureDatabaseExistsAsync(configuration, logger);

                // ✅ Paso 2: Crear tablas y schemas con EF
                var context = services.GetRequiredService<ApplicationDbContext>();
                logger.LogInformation("Creating database schema and tables...");

                await context.Database.EnsureCreatedAsync();

                logger.LogInformation("Database initialization completed successfully!");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing database");
                throw;
            }
        }

        private static async Task EnsureDatabaseExistsAsync(IConfiguration configuration, ILogger logger)
        {
            var connectionString = configuration.GetConnectionString("DBConnectionString");
            var builder = new SqlConnectionStringBuilder(connectionString);
            var databaseName = builder.InitialCatalog;

            // ✅ Conectar a master para crear la DB
            builder.InitialCatalog = "master";
            var masterConnectionString = builder.ConnectionString;

            // ✅ Retry logic para esperar a que SQL Server esté listo
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    using var connection = new SqlConnection(masterConnectionString);
                    await connection.OpenAsync();

                    // Verificar si la base de datos existe
                    var checkDbCommand = new SqlCommand(
                        $"SELECT COUNT(*) FROM sys.databases WHERE name = '{databaseName}'",
                        connection);

                    var exists = (int)await checkDbCommand.ExecuteScalarAsync() > 0;

                    if (!exists)
                    {
                        logger.LogInformation($"Creating database {databaseName}...");

                        var createDbCommand = new SqlCommand(
                            $"CREATE DATABASE [{databaseName}]",
                            connection);

                        await createDbCommand.ExecuteNonQueryAsync();
                        logger.LogInformation($"Database {databaseName} created successfully");
                    }
                    else
                    {
                        logger.LogInformation($"Database {databaseName} already exists");
                    }

                    return; // Success
                }
                catch (Exception ex) when (i < 9) // Retry logic
                {
                    logger.LogWarning($"Attempt {i + 1} failed to connect to SQL Server: {ex.Message}");
                    await Task.Delay(5000); // Wait 5 seconds before retry
                }
            }

            throw new InvalidOperationException("Could not connect to SQL Server after multiple attempts");
        }
    }
}
