using ConfigurationReader.Application.Models;
using ConfigurationReader.Common;

namespace ConfigurationReader.Application.Strategies
{
    public interface IConfigurationFetchStrategy
    {
        Task<ServiceResponse<List<ConfigurationDto>>> GetAllAsync(string? name);
    }
}
