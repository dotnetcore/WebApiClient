using System;
using System.Buffers;
using System.Text.Json;

namespace WebApiClientCore.Serialization
{
    /// <summary>
    /// 默认的json序列化工具
    /// </summary>
    public class JsonSerializer : IJsonSerializer
    {
        /// <summary>
        /// 默认选项
        /// </summary>
        private static readonly JsonSerializerOptions defaultOptions = new JsonSerializerOptions();

        /// <summary>
        /// 将对象序列化为utf8 json到指定的bufferWriter
        /// </summary>
        /// <param name="bufferWriter">buffer写入器</param>
        /// <param name="obj">对象</param>
        /// <param name="options">选项</param>
        public void Serialize(IBufferWriter<byte> bufferWriter, object? obj, JsonSerializerOptions? options)
        {
            if (obj == null)
            {
                return;
            }

            var jsonOptions = options ?? defaultOptions;
            var writerOptions = new JsonWriterOptions
            {
                Indented = jsonOptions.WriteIndented,
                Encoder = jsonOptions.Encoder
            };

            using var utf8JsonWriter = new Utf8JsonWriter(bufferWriter, writerOptions);
            System.Text.Json.JsonSerializer.Serialize(utf8JsonWriter, obj, obj.GetType(), jsonOptions);
        }         

        /// <summary>
        /// 反序列化json为对象
        /// </summary>
        /// <param name="json">json</param>
        /// <param name="objType">对象类型</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        public object? Deserialize(byte[]? json, Type objType, JsonSerializerOptions? options)
        {
            return json == null || json.Length == 0 ?
                objType.DefaultValue() :
                System.Text.Json.JsonSerializer.Deserialize(json, objType, options);
        }
    }
}