using Microsoft.Extensions.Configuration;

namespace ConfigurationReader
{
    public class ConfigurationSource : IConfigurationSource
    {
        private CustomConfigurationProvider? customConfigurationSource;

        public ConfigurationSource(string applicationName)
        {
            this.ApplicationName = applicationName;
        }

        public string ApplicationName { get; }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            this.customConfigurationSource = new CustomConfigurationProvider();
            return this.customConfigurationSource;
        }

        public void Refresh(List<ConfigurationDto> configurations)
        {
            if (this.customConfigurationSource == null)
            {
                return;
            }

            this.customConfigurationSource.Refresh(configurations);
        }

        public CustomConfigurationProvider GetConfigurationProvider()
        {
            if (this.customConfigurationSource == null)
            {
                throw new InvalidOperationException("CustomConfigurationProvider is not set, call ConfigureConfigurationsProvider when building the host!");
            }

            return this.customConfigurationSource;
        }
    }
}