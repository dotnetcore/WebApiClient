using System;

namespace WebApiClientCore.Serialization.JsonConverters
{
    /// <summary>
    /// 表示DateTime和DateTimeOffset的Json转换器
    /// </summary>
    [Obsolete("请使用JsonDateTimeConverter替代")]
    public class JsonLocalDateTimeConverter : JsonDateTimeConverter
    {
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