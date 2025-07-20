namespace ConfigurationReader.Application.Strategies
{
    public class ConfigurationStrategyFactory
    {
        private readonly IConfigurationFetchStrategy nonFilteredConfigurationStrategy;
        private readonly IConfigurationFetchStrategy filteredConfigurationStrategy;

        public ConfigurationStrategyFactory(
            NonFilteredConfigurationStrategy nonFilteredConfigurationStrategy,
            FilteredConfigurationStrategy filteredConfigurationStrategy)
        {
            this.nonFilteredConfigurationStrategy = nonFilteredConfigurationStrategy;
            this.filteredConfigurationStrategy = filteredConfigurationStrategy;
        }

        public IConfigurationFetchStrategy GetStrategy(string? name)
        {
            return string.IsNullOrWhiteSpace(name)
                ? this.nonFilteredConfigurationStrategy
                : this.filteredConfigurationStrategy;
        }
    }
}
