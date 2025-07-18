namespace ConfigurationReader.FeatureProvider
{
    public class FeaturesSyncServiceReporter : IFeaturesSyncServiceReporter
    {
        public string ApplicationName { get; internal set; } = string.Empty;

        public string FeaturesApiUrl { get; internal set; } = string.Empty;

        public int RefreshIntervalInSeconds { get; internal set; }

        public DateTime? LastApiCallTime { get; internal set; }

        public string? LastApiCallResult { get; internal set; }

        public IReadOnlyDictionary<string, string>? Features { get; internal set; }
    }
}