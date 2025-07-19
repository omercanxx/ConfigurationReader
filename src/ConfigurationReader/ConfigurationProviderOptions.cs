namespace ConfigurationReader
{
    public class ConfigurationProviderOptions
    {
        /// <summary>
        /// Default value is assembly name.
        /// </summary>
        public string? ApplicationName { get; set; }

        public string? ConfigurationApiUrl { get; set; }

        /// <summary>
        /// Default value is 0 (no sync).
        /// </summary>
        public int RefreshIntervalInSeconds { get; set; }
    }
}