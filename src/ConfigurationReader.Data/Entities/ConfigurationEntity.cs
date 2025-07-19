using ConfigurationReader.Common.Enums;

namespace ConfigurationReader.Data.Entities
{
    public class ConfigurationEntity
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public FeatureType Type { get; set; }

        public string Value { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public string ApplicationName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
