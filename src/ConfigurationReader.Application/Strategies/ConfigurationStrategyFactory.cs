using ConfigurationReader.Application.Strategies.Interfaces;

namespace ConfigurationReader.Application.Strategies
{
    public class ConfigurationStrategyFactory 
    {
        private readonly IConfigurationStrategy nonFilteredConfigurationStrategy;
        private readonly IConfigurationStrategy filteredConfigurationStrategy;

        public ConfigurationStrategyFactory(
            NonFilteredConfigurationStrategy nonFilteredConfigurationStrategy,
            FilteredConfigurationStrategy filteredConfigurationStrategy)
        {
            this.nonFilteredConfigurationStrategy = nonFilteredConfigurationStrategy;
            this.filteredConfigurationStrategy = filteredConfigurationStrategy;
        }

        public IConfigurationStrategy GetStrategy(string? name)
        {
            return string.IsNullOrWhiteSpace(name)
                ? this.nonFilteredConfigurationStrategy
                : this.filteredConfigurationStrategy;
        }
    }
}
