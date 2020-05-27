using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApiClientCore.Serialization.JsonConverters
{
    /// <summary>
    /// 表示DateTime的本地格式化Json转换器
    /// </summary>
    public class JsonLocalDateTimeConverter : JsonConverter<DateTime>
    {
        /// <summary>
        /// 获取O格式的默认实例
        /// </summary>
        public static JsonLocalDateTimeConverter Default { get; } = new JsonLocalDateTimeConverter();

        /// <summary>
        /// 获取本设备的时间格式的实例
        /// </summary>
        public static JsonLocalDateTimeConverter LocalMachine { get; } = new JsonLocalDateTimeConverter($"{DateTimeFormatInfo.CurrentInfo.ShortDatePattern} {DateTimeFormatInfo.CurrentInfo.LongTimePattern}");

        /// <summary>
        /// 获取时间格式
        /// </summary>
        public string DateTimeFormat { get; }

        /// <summary>
        /// DateTime的本地格式化转换器
        /// </summary>      
        /// <param name="dateTimeFormat">时间格式</param>
        /// <exception cref="ArgumentNullException"></exception>
        public JsonLocalDateTimeConverter(string dateTimeFormat = "O")
        {
            this.DateTimeFormat = dateTimeFormat ?? throw new ArgumentNullException(nameof(dateTimeFormat));
        }


        /// <summary>
        /// 读取时间
        /// 统一转换为本地时间
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = DateTime.Parse(reader.GetString());
            if (value.Kind == DateTimeKind.Local)
            {
                return value;
            }

            if (value.Kind == DateTimeKind.Utc)
            {
                return value.ToLocalTime();
            }

            return DateTime.SpecifyKind(value, DateTimeKind.Local);
        }

        /// <summary>
        /// 写入时间
        /// 统一转换为本地时间，再格式化
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="options"></param>
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            if (value.Kind == DateTimeKind.Utc)
            {
                value = value.ToLocalTime();
            }
            else if (value.Kind == DateTimeKind.Unspecified)
            {
                value = DateTime.SpecifyKind(value, DateTimeKind.Local);
            }

            var dateTimeString = value.ToString(this.DateTimeFormat, CultureInfo.InvariantCulture);
            writer.WriteStringValue(dateTimeString);
        }
    }
}