using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigurationReader
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConfigurationProvider(this IServiceCollection services, IConfiguration? configuration = null)
        {
            IConfigurationSection configSection = configuration.GetSection(nameof(ConfigurationProviderOptions));

            services.AddHostedService<ConfigurationSyncHostedService>()
                .Configure<ConfigurationProviderOptions>(configSection);

            return services;
        }
    }
}