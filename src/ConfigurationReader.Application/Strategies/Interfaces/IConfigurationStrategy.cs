using ConfigurationReader.Application.Models;
using ConfigurationReader.Common;

namespace ConfigurationReader.Application.Strategies.Interfaces
{
    public interface IConfigurationStrategy
    {
        Task<ServiceResponse<List<ConfigurationDto>>> GetAllAsync(string? name);
    }
}
