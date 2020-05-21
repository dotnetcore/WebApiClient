using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace WebApiClientCore.JsonConverters
{
    /// <summary>
    /// JsonString的类型转换器
    /// </summary>
    class JsonStringConverter : JsonConverterFactory
    {
        /// <summary>
        /// 获取唯一实例
        /// </summary>
        public static JsonStringConverter Default { get; } = new JsonStringConverter();

        /// <summary>
        /// 返回是否可以转换
        /// </summary>
        /// <param name="typeToConvert"></param>
        /// <returns></returns>
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsInheritFrom<IJsonString>();
        }

        /// <summary>
        /// 创建转换器
        /// </summary>
        /// <param name="typeToConvert"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return Lambda.CreateCtorFunc<JsonConverter>(typeof(Converter<>).MakeGenericType(typeToConvert))();
        }

        /// <summary>
        /// 转换器
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class Converter<T> : JsonConverter<T>
        {
            /// <summary>
            /// 将json文本反序列化JsonString的Value的类型
            /// 并构建JsonString类型并返回
            /// </summary>
            /// <param name="reader"></param>
            /// <param name="typeToConvert"></param>
            /// <param name="options"></param>
            /// <returns></returns>
            [return: MaybeNull]
            public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                var json = reader.GetString();
                var valueType = typeToConvert.GenericTypeArguments.First();
                var value = JsonSerializer.Deserialize(json, valueType, options);
                return (T)Activator.CreateInstance(typeToConvert, value);
            }

            /// <summary>
            /// 将JsonString的value序列化文本，并作为json的某字段值
            /// </summary>
            /// <param name="writer"></param>
            /// <param name="value"></param>
            /// <param name="options"></param>
            public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
            {
                var jsonString = (IJsonString?)value;
                if (jsonString == null)
                {
                    writer.WriteStringValue(default(string));
                }
                else
                {
                    var json = JsonSerializer.Serialize(jsonString.Value, jsonString.ValueType, options);
                    writer.WriteStringValue(json);
                }
            }
        }
    }
}
