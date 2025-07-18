using Microsoft.Extensions.Hosting;

namespace ConfigurationReader.FeatureProvider
{
    public static class ConfigurationBuilderExtensions
    {
        private static FeaturesConfigurationSource? featuresConfigurationSource;

        public static IHostBuilder ConfigureFeaturesProvider(this IHostBuilder builder)
        {
            return builder.ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
            {
                featuresConfigurationSource = new FeaturesConfigurationSource(hostBuilderContext.HostingEnvironment.ApplicationName);
                configurationBuilder.Add(featuresConfigurationSource);
            });
        }

        public static FeaturesConfigurationSource GetFeaturesConfigurationSource()
        {
            if (featuresConfigurationSource == null)
            {
                throw new InvalidOperationException("FeaturesConfigurationSource is not set, call ConfigureFeaturesProvider when building the host!");
            }

            return featuresConfigurationSource;
        }
    }
}
