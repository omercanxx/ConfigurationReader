namespace ConfigurationReader.Common.Extensions
{
    public static class DateFormatExtensions
    {
        public const string CustomDateFormat = "yyyy-MM-dd";
        public const string CustomDateTimeFormat = "yyyy-MM-dd HH:mm";

        public static string ToDateString(this DateTime dateTime, string dateFormat)
        {
            return dateTime.ToString(dateFormat);
        }

        public static string ToDateString(this DateTime? dateTime, string dateFormat)
        {
            if(dateTime == null)
            {
                return string.Empty;
            }

            return dateTime.Value.ToString(dateFormat);
        }
    }
}
