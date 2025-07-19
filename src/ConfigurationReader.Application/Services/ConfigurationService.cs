using ConfigurationReader.Application.Models;
using ConfigurationReader.Common;
using ConfigurationReader.Common.Extensions;
using ConfigurationReader.Data.Entities;
using ConfigurationReader.Data.Repository;

namespace ConfigurationReader.Application.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IConfigurationRepository configurationRepository;

        public ConfigurationService(IConfigurationRepository configurationRepository)
        {
            this.configurationRepository = configurationRepository;
        }

        public async Task<ServiceResponse<List<ConfigurationDto>>> GetAllAsync(string? name)
        {
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

        public async Task<ServiceResponse<List<ConfigurationDto>>> GetAllByApplicationNameAsync(string applicationName)
        {
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
            return await this.configurationRepository.CreateAsync(new ConfigurationEntity()
            {
                Name = request.Name,
                Type = request.Type,
                Value = request.Value,
                ApplicationName = request.ApplicationName
            });
        }

        public async Task<ServiceResponse> DeleteAsync(int id)
        {
            return await this.configurationRepository.DeleteAsync(id);
        }
    }
}
