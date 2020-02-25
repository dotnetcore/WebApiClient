using System.Globalization;

namespace WebApiClient
{
    /// <summary>
    /// Provide some datetime format templates
    /// </summary>
    public static class DateTimeFormats
    {
        /// <summary>
        /// ISO8601 datetime format accurate to the millisecond
        /// </summary>
        public const string ISO8601_WithMillisecond = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";

        /// <summary>
        /// ISO8601 Date-time format to the second
        /// </summary>
        public const string ISO8601_WithoutMillisecond = "yyyy'-'MM'-'dd'T'HH':'mm':'ssK";

        /// <summary>
        /// Get local datetime format
        /// ShortDate LongTime
        /// </summary>
        public static string LocalDateTimeFormat
        {
            get
            {
                var formatInfo = DateTimeFormatInfo.CurrentInfo;
                return $"{formatInfo.ShortDatePattern} {formatInfo.LongTimePattern}";
            }
        }
    }
}
