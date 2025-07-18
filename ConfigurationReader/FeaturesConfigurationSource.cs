using Microsoft.Extensions.Configuration;

namespace ConfigurationReader.FeatureProvider
{
    public class FeaturesConfigurationSource : IConfigurationSource
    {
        private FeaturesConfigurationProvider? featuresConfigurationProvider;

        public FeaturesConfigurationSource(string applicationName)
        {
            this.ApplicationName = applicationName;
        }

        public string ApplicationName { get; }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            this.featuresConfigurationProvider = new FeaturesConfigurationProvider();
            return this.featuresConfigurationProvider;
        }

        public void Refresh(FeatureDto[] features)
        {
            if (this.featuresConfigurationProvider == null)
            {
                return;
            }

            this.featuresConfigurationProvider.Refresh(features);
        }

        public FeaturesConfigurationProvider GetConfigurationProvider()
        {
            if (this.featuresConfigurationProvider == null)
            {
                throw new InvalidOperationException("FeaturesConfigurationProvider is not set, call ConfigureFeaturesProvider when building the host!");
            }

            return this.featuresConfigurationProvider;
        }
    }
}