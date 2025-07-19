using ConfigurationReader.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConfigurationReader.Data
{
    public interface IAppDbContext
    {
        DbSet<ConfigurationEntity> Configurations { get; }

        Task SaveChangesAsync();
    }
}
