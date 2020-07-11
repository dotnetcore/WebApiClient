using System;
using System.Buffers;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
        ///  将对象序列化为 utf8编码的Json 到指定的bufferWriter
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
                SkipValidation = true,
                Encoder = jsonOptions.Encoder,
                Indented = jsonOptions.WriteIndented
            };

            using var utf8JsonWriter = new Utf8JsonWriter(bufferWriter, writerOptions);
            System.Text.Json.JsonSerializer.Serialize(utf8JsonWriter, obj, obj.GetType(), jsonOptions);
        }

        /// <summary>
        /// 将utf8编码的Json反序列化为对象
        /// </summary>
        /// <param name="utf8Json">json</param>
        /// <param name="objType">对象类型</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        public object? Deserialize(ReadOnlySpan<byte> utf8Json, Type objType, JsonSerializerOptions? options)
        {
            return utf8Json.IsEmpty
                ? objType.DefaultValue()
                : System.Text.Json.JsonSerializer.Deserialize(utf8Json, objType, options);
        }

        /// <summary>
        /// 将utf8编码的Json流 反序列化为对象
        /// </summary>
        /// <param name="utf8JsonStream">utf8编码的Json流</param>
        /// <param name="objType">对象类型</param>
        /// <param name="options">选项</param>
        /// <returns></returns>
        public Task<object> DeserializeAsync(Stream utf8JsonStream, Type objType, JsonSerializerOptions? options)
        {
            return System.Text.Json.JsonSerializer.DeserializeAsync(utf8JsonStream, objType, options).AsTask();
        }
    }
}