using ConfigurationReader.Application.Constants;
using ConfigurationReader.Application.Mappings;
using ConfigurationReader.Application.Models;
using ConfigurationReader.Application.Services.Interfaces;
using ConfigurationReader.Application.Strategies.Interfaces;
using ConfigurationReader.Common;
using ConfigurationReader.Data.Repository;
using Microsoft.Extensions.Logging;

namespace ConfigurationReader.Application.Strategies
{
    public class NonFilteredConfigurationStrategy : IConfigurationStrategy
    {
        private readonly IRedisCacheService redisCacheService;
        private readonly ILogger<NonFilteredConfigurationStrategy> logger;
        private readonly IConfigurationRepository configurationRepository;

        public NonFilteredConfigurationStrategy(
            IRedisCacheService redisCacheService,
            ILogger<NonFilteredConfigurationStrategy> logger,
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

                return new ServiceResponse<List<ConfigurationDto>>(cached);
            }

            var configurations = await this.configurationRepository.GetAllAsync(name);

            var configurationDtos = configurations.Result.ToConfigurationDtoList();

            await this.redisCacheService.SetAsync(CacheKeys.AllConfigurations, configurationDtos.Result, TimeSpan.FromDays(1));

            return configurationDtos;
        }
    }
}