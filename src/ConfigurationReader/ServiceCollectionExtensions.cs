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
                .Configure<ConfigurationProviderOptions>(configSection)
                .AddSingleton<IConfigurationSyncServiceReporter, ConfigurationSyncServiceReporter>();

            ConfigurationProviderOptions? configurationsProviderOptions = configSection.Get<ConfigurationProviderOptions>();

            if (configurationsProviderOptions != null
                && !string.IsNullOrEmpty(configurationsProviderOptions.ConfigurationApiUrl))
            {
                services.AddHttpClient("ConfigurationApi", c => c.BaseAddress = new Uri(configurationsProviderOptions.ConfigurationApiUrl));
            }
            else
            {
                services.AddHttpClient();
            }

            return services;
        }
    }
}