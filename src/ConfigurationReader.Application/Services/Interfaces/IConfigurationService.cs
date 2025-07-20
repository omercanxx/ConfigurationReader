using ConfigurationReader.Application.Models;
using ConfigurationReader.Common;

namespace ConfigurationReader.Application.Services.Interfaces
{
    public interface IConfigurationService
    {
        Task<ServiceResponse<List<ConfigurationDto>>> GetAllAsync(string? name);

        Task<ServiceResponse<List<ConfigurationDto>>> GetAllByApplicationNameAsync(string applicationName);

        Task<ServiceResponse> CreateAsync(CreateConfigurationModel request);

        Task<ServiceResponse> DeleteAsync(int id);
    }
}
