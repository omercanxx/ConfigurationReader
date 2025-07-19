using Microsoft.Extensions.Hosting;

namespace ConfigurationReader
{
    public static class ConfigurationBuilderExtensions
    {
        private static ConfigurationSource? configurationSource;

        public static IHostBuilder ConfigureConfigurationProvider(this IHostBuilder builder)
        {
            return builder.ConfigureAppConfiguration((hostBuilderContext, configurationBuilder) =>
            {
                configurationSource = new ConfigurationSource(hostBuilderContext.HostingEnvironment.ApplicationName);
                configurationBuilder.Add(configurationSource);
            });
        }

        public static ConfigurationSource GetConfigurationSource()
        {
            if (configurationSource == null)
            {
                throw new InvalidOperationException("ConfigurationSource is not set, call ConfigureConfigurationProvider when building the host!");
            }

            return configurationSource;
        }
    }
}
