using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 提供ISO8601格式模块
    /// </summary>
    public static class DateTimeFormats
    {
        /// <summary>
        /// ISO8601精确到毫秒的日期时间格式
        /// </summary>
        public const string ISO8601WithMillisecond = "yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'fffffffzzzz";

        /// <summary>
        /// ISO8601精确到秒的日期时间格式
        /// </summary>
        public const string ISO8601NoneMillisecond = "yyyy'-'MM'-'dd'T'HH':'mm':'sszzzz";

        /// <summary>
        /// 获取本地的时间格式
        /// </summary>
        /// <returns></returns>
        public static string GetLocalDateTimeFormat()
        {
            return DateTimeFormatInfo.CurrentInfo.ShortDatePattern + " " + DateTimeFormatInfo.CurrentInfo.LongTimePattern;
        }
    }
}
