using System;
using System.Globalization;

namespace WebApiClientCore.Serialization.JsonConverters
{
    /// <summary>
    /// 表示DateTime和DateTimeOffset的Json转换器
    /// </summary>
    [Obsolete("请使用JsonDateTimeConverter替代")]
    public class JsonLocalDateTimeConverter : JsonDateTimeConverter
    {
        /// <summary>
        /// 获取ISO8601格式的实例
        /// </summary>
        public static JsonLocalDateTimeConverter Default { get; } = new JsonLocalDateTimeConverter();

        /// <summary>
        /// 获取本设备的时间格式的实例
        /// </summary>
        public static JsonLocalDateTimeConverter LocalMachine { get; } = new JsonLocalDateTimeConverter($"{DateTimeFormatInfo.CurrentInfo.ShortDatePattern} {DateTimeFormatInfo.CurrentInfo.LongTimePattern}");


        /// <summary>
        /// DateTime和DateTimeOffset的Json转换器
        /// </summary>      
        /// <param name="dateTimeFormat">日期时间格式</param>
        /// <exception cref="ArgumentNullException"></exception>
        public JsonLocalDateTimeConverter(string dateTimeFormat = "O")
            : base(dateTimeFormat)
        {
        }
    }
}