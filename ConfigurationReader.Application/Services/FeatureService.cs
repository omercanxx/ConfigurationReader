using ConfigurationReader.Application.Models;
using ConfigurationReader.Data.Entities;
using ConfigurationReader.Data.Repository;

namespace ConfigurationReader.Application.Services
{
    public class FeatureService : IFeatureService
    {
        private readonly IFeatureRepository featureRepository;

        public FeatureService(IFeatureRepository featureRepository)
        {
            this.featureRepository = featureRepository;
        }

        public async Task<List<FeatureDto>> GetAllAsync()
        {
            var features = await this.featureRepository.GetAllAsync();

            return features.Select(f => new FeatureDto
            {
                Id = f.Id,
                Name = f.Name,
                Type = f.Type.ToString(),
                Value = f.Value,
                IsActive = f.IsActive,
                ApplicationName = f.ApplicationName,
                CreatedAt = f.CreatedAt,
                UpdatedAt = f.UpdatedAt
            }).ToList();
        }

        public async Task CreateAsync(CreateFeatureModel request)
        {
            try
            {
                await this.featureRepository.CreateAsync(new FeatureEntity()
                {
                    Name = request.Name,
                    Type = request.Type,
                    Value = request.Value,
                    ApplicationName = request.ApplicationName
                });
            }
            catch(Exception e)
            {

            }
        }
    }
}
