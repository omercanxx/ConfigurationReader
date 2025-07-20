using ConfigurationReader.Application.Services;
using ConfigurationReader.Application.Services.Interfaces;
using ConfigurationReader.Application.Strategies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigurationReader.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddRedis(configuration)
                .AddStrategyFactory()
                .AddScoped<IConfigurationService, ConfigurationService>();
        }

        private static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
            });

            return services
                .AddScoped<IRedisCacheService, RedisCacheService>();
        }

        private static IServiceCollection AddStrategyFactory(this IServiceCollection services)
        {
            return services
                .AddScoped<NonFilteredConfigurationStrategy>()
                .AddScoped<FilteredConfigurationStrategy>()
                .AddScoped<ConfigurationStrategyFactory>();
        }
    }
}
