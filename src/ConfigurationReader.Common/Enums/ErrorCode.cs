using System.ComponentModel;

namespace ConfigurationReader.Common.Enums
{
    public enum ErrorCode
    {
        [Description("Bir hata oluştu")]
        Default = 100,

        [Description("Config bulunamadı")]
        ConfigurationNotFound = 101
    }
}
