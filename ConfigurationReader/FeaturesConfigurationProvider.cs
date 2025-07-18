using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace ConfigurationReader.FeatureProvider
{
    public class FeaturesConfigurationProvider : ConfigurationProvider
    {
        private const string SectionName = "FeatureManagement";
        private Dictionary<string, string>? data;

        public override void Load()
        {
            if (this.data == null)
            {
                return;
            }

            this.Data = this.data;
            this.OnReload();
        }

        public void Refresh(FeatureDto[] features)
        {
            this.data = features.ToDictionary(f => SectionName + ":" + f.Name, f => f.Value.ToString(CultureInfo.InvariantCulture));
            this.Load();
        }
    }
}