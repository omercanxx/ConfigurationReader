using ConfigurationReader.Common.Extensions;
using ConfigurationReader.Data;
using Microsoft.EntityFrameworkCore;

namespace ConfigurationReader
{
    public class ConfigurationReader
    {
        public string ApplicationName { get; internal set; } = string.Empty;

        public string ConnectionString { get; internal set; } = string.Empty;

        public int RefreshIntervalInSeconds { get; internal set; }

        public DateTime? LastApiCallTime { get; internal set; }

        public string? LastApiCallResult { get; internal set; }

        public IReadOnlyDictionary<string, string>? Configurations { get; internal set; }

        public ConfigurationReader(string applicationName, string connectionString, int refreshIntervalInSeconds)
        {
            this.ApplicationName = applicationName;
            this.ConnectionString = connectionString;
            this.RefreshIntervalInSeconds = refreshIntervalInSeconds;
        }

        public async Task<T> GetValueAsync<T>(string key)
            where T : class
        {
            using var dbContext = new AppDbContext(this.ConnectionString);

            var value = await dbContext.Configurations
                .Where(x => x.ApplicationName == this.ApplicationName && x.Name == key && x.IsActive)
                .FirstOrDefaultAsync();

            return (T)(object)value.Value;
        }

        public async Task<List<ConfigurationDto>> GetValuesAsync()
        {
            using var dbContext = new AppDbContext(this.ConnectionString);

            var entities =  await dbContext.Configurations
                .Where(x => x.ApplicationName == this.ApplicationName && x.IsActive)
                .ToListAsync();

            return entities.Select(x => new ConfigurationDto
            {
                Id = x.Id,
                Name = x.Name,
                Type = x.Type.ToString(),
                Value = x.Value,
                IsActive = x.IsActive,
                ApplicationName = x.ApplicationName,
                CreatedAt = x.CreatedAt.ToDateString(DateFormatExtensions.CustomDateTimeFormat),
                UpdatedAt = x.UpdatedAt.ToDateString(DateFormatExtensions.CustomDateTimeFormat)
            }).ToList();
        }
    }
}