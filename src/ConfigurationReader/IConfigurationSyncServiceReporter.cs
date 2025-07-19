namespace ConfigurationReader
{
    public interface IConfigurationSyncServiceReporter
    {
        string ApplicationName { get; }

        public string ConfigurationApiUrl { get; }

        public int RefreshIntervalInSeconds { get; }

        DateTime? LastApiCallTime { get; }

        public string? LastApiCallResult { get; }

        IReadOnlyDictionary<string, string>? Configurations { get; }
    }
}