namespace ConfigurationReader.FeatureProvider
{
    public interface IFeaturesSyncServiceReporter
    {
        string ApplicationName { get; }

        public string FeaturesApiUrl { get; }

        public int RefreshIntervalInSeconds { get; }

        DateTime? LastApiCallTime { get; }

        public string? LastApiCallResult { get; }

        IReadOnlyDictionary<string, string>? Features { get; }
    }
}