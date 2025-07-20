using ConfigurationReader.Application.Constants;
using ConfigurationReader.Application.Models;
using ConfigurationReader.Application.Strategies;
using ConfigurationReader.Common;
using ConfigurationReader.Common.Extensions;
using ConfigurationReader.Data.Entities;
using ConfigurationReader.Data.Repository;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ConfigurationReader.Application.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly ConfigurationStrategyFactory configurationFetchStrategyFactory;
        private readonly ILogger<ConfigurationService> logger;
        private readonly IDistributedCache distributedCache;
        private readonly IConfigurationRepository configurationRepository;

        public ConfigurationService(
            ConfigurationStrategyFactory configurationFetchStrategyFactory,
            ILogger<ConfigurationService> logger,
            IDistributedCache distributedCache,
            IConfigurationRepository configurationRepository)
        {
            this.configurationFetchStrategyFactory = configurationFetchStrategyFactory;
            this.logger = logger;
            this.distributedCache = distributedCache;
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
            var cachedResult = await this.distributedCache.GetStringAsync(CacheKeys.AllConfigurations);

            if (cachedResult != null)
            {
                this.logger.LogInformation("Configurations retrieved from cache!");

                var items = JsonConvert.DeserializeObject<List<ConfigurationDto>>(cachedResult);

                return new ServiceResponse<List<ConfigurationDto>>(items?.Where(x => x.ApplicationName == applicationName).ToList());
            }

            var configurations = await this.configurationRepository.GetAllByApplicationNameAsync(applicationName);

            return new ServiceResponse<List<ConfigurationDto>>(configurations.Result.Select(f => new ConfigurationDto
            {
                Id = f.Id,
                Name = f.Name,
                Type = f.Type.ToString(),
                Value = f.Value,
                IsActive = f.IsActive,
                ApplicationName = f.ApplicationName,
                CreatedAt = f.CreatedAt.ToDateString(DateFormatExtensions.CustomDateTimeFormat),
                UpdatedAt = f.UpdatedAt.ToDateString(DateFormatExtensions.CustomDateTimeFormat)
            }).ToList());
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

            await this.distributedCache.RemoveAsync(CacheKeys.AllConfigurations);

            return new ServiceResponse();
        }

        public async Task<ServiceResponse> DeleteAsync(int id)
        {
            await this.configurationRepository.DeleteAsync(id);

            await this.distributedCache.RemoveAsync(CacheKeys.AllConfigurations);

            return new ServiceResponse();
        }
    }
}
