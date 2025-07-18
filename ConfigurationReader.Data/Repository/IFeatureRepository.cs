using ConfigurationReader.Data.Entities;

namespace ConfigurationReader.Data.Repository
{
    public interface IFeatureRepository
    {
        Task<List<FeatureEntity>> GetAllAsync();

        Task CreateAsync(FeatureEntity entity);
    }
}
