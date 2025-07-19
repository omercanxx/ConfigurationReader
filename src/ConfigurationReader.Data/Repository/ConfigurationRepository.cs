using ConfigurationReader.Common;
using ConfigurationReader.Common.Enums;
using ConfigurationReader.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConfigurationReader.Data.Repository
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        private readonly IAppDbContext appDbContext;

        public ConfigurationRepository(IAppDbContext appDbContext)
        {
            this.appDbContext = appDbContext;
        }

        public async Task<ServiceResponse<List<ConfigurationEntity>>> GetAllAsync(string? name)
        {
            var query = this.appDbContext.Configurations
                .Where(x => x.IsActive);

            if(!string.IsNullOrEmpty(name))
            {
                query = query.Where(x => x.Name == name);
            }

            return new ServiceResponse<List<ConfigurationEntity>>(await query.ToListAsync());
        }

        public async Task<ServiceResponse<List<ConfigurationEntity>>> GetAllByApplicationNameAsync(string applicationName)
        {
            var entities = await this.appDbContext.Configurations
                .Where(x => x.ApplicationName == applicationName && x.IsActive).ToListAsync();

            return new ServiceResponse<List<ConfigurationEntity>>(entities);
        }

        public async Task<ServiceResponse> CreateAsync(ConfigurationEntity entity)
        {
            await this.appDbContext.Configurations.AddAsync(entity);

            await this.appDbContext.SaveChangesAsync();

            return new ServiceResponse();
        }

        public async Task<ServiceResponse> DeleteAsync(int id)
        {
            var entity = await this.appDbContext.Configurations.Where(x => x.Id == id && x.IsActive).FirstOrDefaultAsync();

            if(entity == null)
            {
                return new ServiceResponse(ErrorCode.ConfigurationNotFound);
            }

            entity.IsActive = false;

            entity.UpdatedAt = DateTime.UtcNow;

            await this.appDbContext.SaveChangesAsync();

            return new ServiceResponse();
        }
    }
}
