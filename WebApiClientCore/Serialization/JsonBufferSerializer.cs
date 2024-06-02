using System.Buffers;
using System.Text.Json;

namespace WebApiClientCore.Serialization
{
    /// <summary>
    /// json序列化工具
    /// </summary>
    public static class JsonBufferSerializer
    {
        /// <summary>
        /// 默认选项
        /// </summary>
        private static readonly JsonSerializerOptions defaultOptions = new();

        /// <summary>
        /// 将对象序列化为utf8编码的Json到指定的bufferWriter
        /// </summary>
        /// <param name="bufferWriter">buffer写入器</param>
        /// <param name="obj">对象</param>
        /// <param name="options">选项</param>
        public static void Serialize(IBufferWriter<byte> bufferWriter, object? obj, JsonSerializerOptions? options)
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
            JsonSerializer.Serialize(utf8JsonWriter, obj, obj.GetType(), jsonOptions);
        }
    }
}