using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApiClientCore.Serialization.JsonConverters
{
    /// <summary>
    /// 表示指定时期时间格式的Json转换器
    /// 支持DateTime和DateTimeOffset
    /// </summary>
    public class JsonDateTimeConverter : JsonConverterFactory
    {
        /// <summary>
        /// 获取ISO8601格式的实例
        /// </summary>
        public static JsonDateTimeConverter Default { get; } = new JsonDateTimeConverter("O");

        /// <summary>
        /// 获取本设备的时间格式的实例
        /// </summary>
        public static JsonDateTimeConverter LocalMachine { get; } = new JsonDateTimeConverter($"{DateTimeFormatInfo.CurrentInfo.ShortDatePattern} {DateTimeFormatInfo.CurrentInfo.LongTimePattern}");


        /// <summary>
        /// 获取日期时间格式
        /// </summary>
        public string DateTimeFormat { get; }

        /// <summary>
        /// 指定时期时间格式的Json转换器
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <param name="dateTimeFormat">日期时间格式</param>
        public JsonDateTimeConverter(string dateTimeFormat)
        {
            this.DateTimeFormat = dateTimeFormat ?? throw new ArgumentNullException(nameof(dateTimeFormat)); ;
        }

        /// <summary>
        /// 是否能转换
        /// </summary>
        /// <param name="typeToConvert"></param>
        /// <returns></returns>
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(DateTime) || typeToConvert == typeof(DateTimeOffset);
        }

        /// <summary>
        /// 创建转换器
        /// </summary>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            if (typeToConvert == typeof(DateTime))
            {
                return new DateTimeConverter(this.DateTimeFormat);
            }
            else
            {
                return new DateTimeOffsetConverter(this.DateTimeFormat);
            }
        }

        /// <summary>
        /// DateTime转换器
        /// </summary>
        private class DateTimeConverter : JsonConverter<DateTime>
        {
            private readonly string format;

            public DateTimeConverter(string format)
            {
                this.format = format;
            }

            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    return DateTime.Parse(reader.GetString());
                }
                return reader.GetDateTime();
            }

            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
            {
                var dateTimeString = value.ToString(this.format, CultureInfo.InvariantCulture);
                writer.WriteStringValue(dateTimeString);
            }
        }

        /// <summary>
        /// DateTimeOffset转换器
        /// </summary>
        private class DateTimeOffsetConverter : JsonConverter<DateTimeOffset>
        {
            private readonly string format;

            public DateTimeOffsetConverter(string format)
            {
                this.format = format;
            }

            public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    return DateTimeOffset.Parse(reader.GetString());
                }
                return reader.GetDateTimeOffset();
            }

            public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
            {
                var dateTimeString = value.ToString(this.format, CultureInfo.InvariantCulture);
                writer.WriteStringValue(dateTimeString);
            }
        }
    }
}
