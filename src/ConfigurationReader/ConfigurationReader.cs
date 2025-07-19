using ConfigurationReader.Data;
using Microsoft.EntityFrameworkCore;

namespace ConfigurationReader
{
    public class ConfigurationReader
    {
        private readonly string applicationName;
        private readonly string connectionString;
        private readonly int refreshTimerIntervalInMs;

        public ConfigurationReader(string applicationName, string connectionString, int refreshTimerIntervalInMs)
        {
            this.applicationName = applicationName;
            this.connectionString = connectionString;
            this.refreshTimerIntervalInMs = refreshTimerIntervalInMs;
        }

        public async Task<T> GetValueAsync<T>(string key)
            where T : class
        {
            using var dbContext = new AppDbContext(this.connectionString);

            var value = await dbContext.Configurations
                .Where(x => x.ApplicationName == this.applicationName && x.Name == key && x.IsActive)
                .FirstOrDefaultAsync();

            return (T)(object)value.Value;
        }
    }
}
