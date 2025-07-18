using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ConfigurationReader.FeatureProvider
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddFeaturesProvider(this IServiceCollection services, IConfiguration? configuration = null)
        {
            IConfigurationSection configSection = configuration.GetSection("FeaturesProviderOptions");

            services.AddHostedService<FeaturesSyncHostedService>()
                .Configure<FeaturesProviderOptions>(configSection)
                .AddSingleton<IFeaturesSyncServiceReporter, FeaturesSyncServiceReporter>();

            return services;
        }
    }
}