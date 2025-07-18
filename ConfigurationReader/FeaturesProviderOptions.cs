namespace ConfigurationReader.FeatureProvider
{
    public class FeaturesProviderOptions
    {
        /// <summary>
        /// Default value is assembly name.
        /// </summary>
        public string? ApplicationName { get; set; }

        public string? FeaturesApiUrl { get; set; }

        /// <summary>
        /// Default value is 0 (no sync).
        /// </summary>
        public int RefreshIntervalInSeconds { get; set; }
    }
}