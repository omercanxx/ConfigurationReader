using ConfigurationReader.Application.Constants;
using ConfigurationReader.Application.Models;
using ConfigurationReader.Application.Mappings;
using ConfigurationReader.Application.Services.Interfaces;
using ConfigurationReader.Application.Strategies;
using ConfigurationReader.Common;
using ConfigurationReader.Data.Entities;
using ConfigurationReader.Data.Repository;
using Microsoft.Extensions.Logging;

namespace ConfigurationReader.Application.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly ConfigurationStrategyFactory configurationFetchStrategyFactory;
        private readonly ILogger<ConfigurationService> logger;
        private readonly IRedisCacheService redisCacheService;
        private readonly IConfigurationRepository configurationRepository;

        public ConfigurationService(
            IRedisCacheService redisCacheService,
            ILogger<ConfigurationService> logger,
            ConfigurationStrategyFactory configurationFetchStrategyFactory,
            IConfigurationRepository configurationRepository)
        {
            this.redisCacheService = redisCacheService;
            this.logger = logger;
            this.configurationFetchStrategyFactory = configurationFetchStrategyFactory;
            this.configurationRepository = configurationRepository;
        }

        public async Task<ServiceResponse<List<ConfigurationDto>>> GetAllAsync(string? name)
        {
            return await this.configurationFetchStrategyFactory
                .GetStrategy(name)
                .GetAllAsync(name);
        }


        public async Task<ServiceResponse<List<ConfigurationDto>>> GetAllByApplicationNameAsync(string applicationName)
        {
            var cachedResult = await this.redisCacheService.GetAsync<List<ConfigurationDto>>(CacheKeys.AllConfigurations);

            if (cachedResult != null)
            {
                this.logger.LogInformation("Configurations retrieved from cache!");

                return new ServiceResponse<List<ConfigurationDto>>(cachedResult.Where(x => x.ApplicationName == applicationName).ToList());
            }

            var configurations = await this.configurationRepository.GetAllByApplicationNameAsync(applicationName);

            return configurations.Result.ToConfigurationDtoList();
        }

        public async Task<ServiceResponse> CreateAsync(CreateConfigurationModel request)
        {
            await this.configurationRepository.CreateAsync(new ConfigurationEntity()
            {
                Name = request.Name,
                Type = request.Type,
                Value = request.Value,
                ApplicationName = request.ApplicationName
            });

            await this.redisCacheService.RemoveAsync(CacheKeys.AllConfigurations);

            return new ServiceResponse();
        }

        public async Task<ServiceResponse> DeleteAsync(int id)
        {
            await this.configurationRepository.DeleteAsync(id);

            await this.redisCacheService.RemoveAsync(CacheKeys.AllConfigurations);

            return new ServiceResponse();
        }
    }
}
