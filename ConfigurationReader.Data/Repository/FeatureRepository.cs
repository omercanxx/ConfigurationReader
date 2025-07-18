using ConfigurationReader.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConfigurationReader.Data.Repository
{
    public class FeatureRepository : IFeatureRepository
    {
        private readonly IAppDbContext appDbContext;

        public FeatureRepository(IAppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<List<FeatureEntity>> GetAllAsync()
        {
            return await this.appDbContext.Features.ToListAsync();
        }

        public async Task CreateAsync(FeatureEntity entity)
        {
            await this.appDbContext.Features.AddAsync(entity);

            await this.appDbContext.SaveChangesAsync();
        }
    }
}
