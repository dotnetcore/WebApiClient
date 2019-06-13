using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WebApiClient.Defaults
{
    /// <summary>
    /// 定义JsonString的接口
    /// </summary>
    interface IJsonString
    {
        /// <summary>
        /// 获取值
        /// </summary>
        object Value { get; }
    }

    /// <summary>
    /// JsonString的类型转换器
    /// </summary>
    class JsonStringConverter : JsonConverter
    {
        /// <summary>
        /// 获取唯一实例
        /// </summary>
        public static JsonStringConverter Instance { get; } = new JsonStringConverter();

        /// <summary>
        /// 是不支持转换
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns></returns>
        public override bool CanConvert(Type objectType)
        {
            return objectType.IsInheritFrom<IJsonString>();
        }

        /// <summary>
        /// 将json文本序列化JsonString的Value的类型
        /// 并构建JsonString类型并返回
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="objectType"></param>
        /// <param name="existingValue"></param>
        /// <param name="serializer"></param>
        /// <returns></returns>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var stringReader = new StringReader(reader.Value?.ToString());
            using (var jsonReader = new JsonTextReader(stringReader))
            {
                var valueType = objectType.GetTypeInfo().GenericTypeArguments.First();
                var value = serializer.Deserialize(jsonReader, valueType);
                var jsonValue = Activator.CreateInstance(objectType, value);
                return jsonValue;
            }
        }


        /// <summary>
        /// 将JsonString的value序列化文本，并作为json的某字段值
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="value"></param>
        /// <param name="serializer"></param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var builder = new StringBuilder();
            using (var jsonWriter = new JsonTextWriter(new StringWriter(builder)))
            {
                serializer.Serialize(jsonWriter, ((IJsonString)value).Value);
                var jsonText = builder.ToString();
                writer.WriteValue(jsonText);
            }
        }
    }
}
