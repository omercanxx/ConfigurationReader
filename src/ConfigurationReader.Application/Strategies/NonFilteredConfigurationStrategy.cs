using ConfigurationReader.Application.Constants;
using ConfigurationReader.Application.Models;
using ConfigurationReader.Common;
using ConfigurationReader.Common.Extensions;
using ConfigurationReader.Data.Entities;
using ConfigurationReader.Data.Repository;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ConfigurationReader.Application.Strategies
{
    public class NonFilteredConfigurationStrategy : IConfigurationFetchStrategy
    {
        private readonly IDistributedCache distributedCache;
        private readonly ILogger<NonFilteredConfigurationStrategy> logger;
        private readonly IConfigurationRepository configurationRepository;

        public NonFilteredConfigurationStrategy(
            IDistributedCache distributedCache,
            ILogger<NonFilteredConfigurationStrategy> logger,
            IConfigurationRepository configurationRepository)
        {
            this.distributedCache = distributedCache;
            this.logger = logger;
            this.configurationRepository = configurationRepository;
        }

        public async Task<ServiceResponse<List<ConfigurationDto>>> GetAllAsync(string? name)
        {
            var cached = await this.distributedCache.GetStringAsync(CacheKeys.AllConfigurations);

            if (string.IsNullOrEmpty(cached))
            {
                var configurations = await this.configurationRepository.GetAllAsync(name);
                
                await SetToCache(configurations);

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

            this.logger.LogInformation("Configurations retrieved from cache!");

            var result = JsonConvert.DeserializeObject<ServiceResponse<List<ConfigurationDto>>>(cached);

            if (result == null)
            {
                return new ServiceResponse<List<ConfigurationDto>>(new List<ConfigurationDto>());
            }

            return result;
        }

        private async Task SetToCache(ServiceResponse<List<ConfigurationEntity>> configurations)
        {
            await this.distributedCache.SetStringAsync(
                CacheKeys.AllConfigurations,
                JsonConvert.SerializeObject(configurations),
                new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromDays(1)));

            this.logger.LogInformation("Configurations set to cache!");
        }
    }
}