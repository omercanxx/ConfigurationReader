using ConfigurationReader.Application.Models;
using ConfigurationReader.Common.Extensions;
using ConfigurationReader.Common;
using ConfigurationReader.Data.Repository;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ConfigurationReader.Application.Constants;

namespace ConfigurationReader.Application.Strategies
{
    public class FilteredConfigurationStrategy : IConfigurationFetchStrategy
    {
        private readonly IConfigurationRepository configurationRepository;
        private readonly IDistributedCache distributedCache;
        private readonly ILogger<FilteredConfigurationStrategy> logger;

        public FilteredConfigurationStrategy(
            IConfigurationRepository configurationRepository,
            IDistributedCache distributedCache,
            ILogger<FilteredConfigurationStrategy> logger)
        {
            this.configurationRepository = configurationRepository;
            this.distributedCache = distributedCache;
            this.logger = logger;
        }

        public async Task<ServiceResponse<List<ConfigurationDto>>> GetAllAsync(string? name)
        {
            var cached = await this.distributedCache.GetStringAsync(CacheKeys.AllConfigurations);

            if (!string.IsNullOrEmpty(cached))
            {
                var result = JsonConvert.DeserializeObject<ServiceResponse<List<ConfigurationDto>>>(cached);

                this.logger.LogInformation("Configurations retrieved from cache!");

                if (result == null)
                {
                    return new ServiceResponse<List<ConfigurationDto>>(new List<ConfigurationDto>());
                }

                // filter the cached results by name
                result.Result = result.Result
                    .Where(x => x.Name.Contains(name, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                return result;
            }

            var configurations = await this.configurationRepository.GetAllAsync(name);

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
    }
}
