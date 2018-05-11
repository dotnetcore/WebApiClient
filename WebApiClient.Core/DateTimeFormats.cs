using System.Globalization;

namespace WebApiClient
{
    /// <summary>
    /// 提供一些日期时间格式模板
    /// </summary>
    public static class DateTimeFormats
    {
        /// <summary>
        /// ISO8601精确到毫秒的日期时间格式
        /// </summary>
        public const string ISO8601_WithMillisecond = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";

        /// <summary>
        /// ISO8601精确到秒的日期时间格式
        /// </summary>
        public const string ISO8601_WithoutMillisecond = "yyyy'-'MM'-'dd'T'HH':'mm':'ssK";

        /// <summary>
        /// 获取本地的日期时间格式
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
