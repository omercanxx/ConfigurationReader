using ConfigurationReader.Common.Enums;

namespace ConfigurationReader.Application.Models
{
    public class CreateConfigurationModel
    {
        public string Name { get; set; }
        
        public FeatureType Type { get; set; }
        
        public string Value { get; set; }
        
        public string ApplicationName { get; set; }
    }
}
