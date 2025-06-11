using TwitterAPI.Domain.Entities;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TwitterAPI.Domain.Entities.Tweet;
using Infrastructure.Cache;
using TwittetAPI.Domain.Abstractions;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {

            // BD
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DBConnectionString"));
            });


            services.AddMemoryCache(options =>
            {
                options.SizeLimit = 1000;
                options.CompactionPercentage = 0.25;
            });

            // Cache Service
            services.AddScoped<ICacheService, MemoryCacheService>();

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserFollowRepository, UserFollowRepository>();
            services.AddScoped<ITweetRepository, TweetRepository>();

            return services;
        }
    }
}
