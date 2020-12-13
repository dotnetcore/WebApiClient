using System;
using System.Text.Json.Serialization;

namespace WebApiClientCore.Serialization.JsonConverters
{
    /// <summary>
    /// 表示指定属性的日期时间格式
    /// 支持DateTime和DateTimeOffset类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class JsonDateTimeAttribute : JsonConverterAttribute
    {
        /// <summary>
        /// 日期时间格式
        /// </summary>
        private readonly string dateTimeFormat;

        /// <summary>
        /// 指定属性的日期时间格式
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public JsonDateTimeAttribute(string dateTimeFormat)
        {
            this.dateTimeFormat = dateTimeFormat ?? throw new ArgumentNullException(nameof(dateTimeFormat));
        }

        /// <summary>
        /// 创建转换器
        /// </summary>
        /// <param name="typeToConvert"></param>
        /// <returns></returns>
        public override JsonConverter CreateConverter(Type typeToConvert)
        {
            var converter = new JsonDateTimeConverter(this.dateTimeFormat);
            if (converter.CanConvert(typeToConvert) == false)
            {
                throw new NotSupportedException($"JsonDateTimeAttribute不支持转换{typeToConvert}");
            }
            return converter;
        }
    }
}
