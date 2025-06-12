using TwitterAPI.Domain.Entities;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TwitterAPI.Domain.Entities.Tweet;
using Infrastructure.Cache;
using Domain.Interfaces;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {

            var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
                                 ?? configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Connection string not found. Set DATABASE_URL environment variable or DefaultConnection in appsettings.");
            }

            connectionString = ConvertDatabaseUrl(connectionString);

            // BD
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });


            services.AddMemoryCache(options =>
            {
                options.SizeLimit = 1000;
                options.CompactionPercentage = 0.25;
            });

            // Cache Service
            services.AddSingleton<ICacheService, MemoryCacheService>();

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserFollowRepository, UserFollowRepository>();
            services.AddScoped<ITweetRepository, TweetRepository>();

            return services;
        }

        // This use is only for railway
        private static string ConvertDatabaseUrl(string databaseUrl)
        {
            if (!databaseUrl.StartsWith("postgresql://") && !databaseUrl.StartsWith("postgres://"))
            {
                return databaseUrl;
            }

            try
            {
                var uri = new Uri(databaseUrl);
                var host = uri.Host;
                var port = uri.Port;
                var database = uri.AbsolutePath.Trim('/');
                var userInfo = uri.UserInfo.Split(':');
                var username = userInfo[0];
                var password = userInfo.Length > 1 ? userInfo[1] : "";

                return $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
            }
            catch (Exception)
            {
                return databaseUrl;
            }
        }
    }
}
