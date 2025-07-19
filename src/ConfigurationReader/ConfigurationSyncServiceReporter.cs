namespace ConfigurationReader
{
    public class ConfigurationSyncServiceReporter : IConfigurationSyncServiceReporter
    {
        public string ApplicationName { get; internal set; } = string.Empty;

        public string ConfigurationApiUrl { get; internal set; } = string.Empty;

        public int RefreshIntervalInSeconds { get; internal set; }

        public DateTime? LastApiCallTime { get; internal set; }

        public string? LastApiCallResult { get; internal set; }

        public IReadOnlyDictionary<string, string>? Configurations { get; internal set; }
    }
}