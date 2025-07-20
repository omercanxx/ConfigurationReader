using ConfigurationReader.Application.Models;
using ConfigurationReader.Common;
using ConfigurationReader.Data.Repository;
using Microsoft.Extensions.Logging;
using ConfigurationReader.Application.Constants;
using ConfigurationReader.Application.Strategies.Interfaces;
using ConfigurationReader.Application.Services.Interfaces;
using ConfigurationReader.Application.Mappings;

namespace ConfigurationReader.Application.Strategies
{
    public class FilteredConfigurationStrategy : IConfigurationStrategy
    {
        private readonly IConfigurationRepository configurationRepository;
        private readonly IRedisCacheService redisCacheService;
        private readonly ILogger<FilteredConfigurationStrategy> logger;

        public FilteredConfigurationStrategy(
            IRedisCacheService redisCacheService,
            ILogger<FilteredConfigurationStrategy> logger,
            IConfigurationRepository configurationRepository)
        {
            this.redisCacheService = redisCacheService;
            this.logger = logger;
            this.configurationRepository = configurationRepository;
        }

        public async Task<ServiceResponse<List<ConfigurationDto>>> GetAllAsync(string? name)
        {
            var cached = await this.redisCacheService.GetAsync<List<ConfigurationDto>>(CacheKeys.AllConfigurations);

            if (cached != null)
            {
                this.logger.LogInformation("Configurations retrieved from cache!");

                // filter the cached results by name
                return new ServiceResponse<List<ConfigurationDto>>(cached
                    .Where(x => x.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                    .ToList());
            }

            var configurations = await this.configurationRepository.GetAllAsync(name);

            return configurations.Result.ToConfigurationDtoList();
        }
    }
}
