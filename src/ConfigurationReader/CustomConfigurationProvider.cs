using Microsoft.Extensions.Configuration;
using System.Globalization;

namespace ConfigurationReader
{
    public class CustomConfigurationProvider : ConfigurationProvider
    {
        private const string SectionName = "ConfigurationManagement";
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

        public void Refresh(List<ConfigurationDto> configurations)
        {
            this.data = configurations.ToDictionary(f => SectionName + ":" + f.Name, f => f.Value.ToString(CultureInfo.InvariantCulture));
            this.Load();
        }
    }
}