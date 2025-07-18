using ConfigurationReader.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConfigurationReader.Data
{
    public interface IAppDbContext
    {
        DbSet<FeatureEntity> Features { get; }

        Task SaveChangesAsync();
    }
}
