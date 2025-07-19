using ConfigurationReader.Common;
using ConfigurationReader.Data.Entities;

namespace ConfigurationReader.Data.Repository
{
    public interface IConfigurationRepository
    {
        Task<ServiceResponse<List<ConfigurationEntity>>> GetAllAsync(string? name);

        Task<ServiceResponse<List<ConfigurationEntity>>> GetAllByApplicationNameAsync(string applicationName);
        
        Task<ServiceResponse> CreateAsync(ConfigurationEntity entity);

        Task<ServiceResponse> DeleteAsync(int id);
    }
}
