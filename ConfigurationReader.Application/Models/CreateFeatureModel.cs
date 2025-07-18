using ConfigurationReader.Data.Enums;

namespace ConfigurationReader.Application.Models
{
    public class CreateFeatureModel
    {
        public string Name { get; set; }
        
        public FeatureType Type { get; set; }
        
        public string Value { get; set; }
        
        public string ApplicationName { get; set; }
    }
}
