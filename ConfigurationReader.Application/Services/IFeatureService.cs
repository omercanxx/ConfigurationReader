using ConfigurationReader.Application.Models;

namespace ConfigurationReader.Application.Services
{
    public interface IFeatureService
    {
        Task<List<FeatureDto>> GetAllAsync();

        Task CreateAsync(CreateFeatureModel request);
    }
}
