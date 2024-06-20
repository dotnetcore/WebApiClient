using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

namespace WebApiClientCore.Serialization
{
    /// <summary>
    /// json序列化工具
    /// </summary>
    public static class JsonBufferSerializer
    {
        [ThreadStatic]
        private static Utf8JsonWriter? bufferWriterUtf8JsonWriter;

        /// <summary>
        /// 默认选项
        /// </summary>
        private static readonly JsonSerializerOptions defaultOptions = new();

        /// <summary>
        /// 将对象序列化为Utf8编码的Json到指定的BufferWriter
        /// </summary>
        /// <param name="bufferWriter">buffer写入器</param>
        /// <param name="obj">对象</param>
        /// <param name="options">选项</param>
        [RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
        [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
        public static void Serialize(IBufferWriter<byte> bufferWriter, object? obj, JsonSerializerOptions? options)
        {
            if (obj == null)
            {
                return;
            }

            options ??= defaultOptions;
            var utf8JsonWriter = bufferWriterUtf8JsonWriter;
            if (utf8JsonWriter == null)
            {
                utf8JsonWriter = new Utf8JsonWriter(bufferWriter, GetJsonWriterOptions(options));
                bufferWriterUtf8JsonWriter = utf8JsonWriter;
            }
            else if (OptionsEquals(utf8JsonWriter.Options, options))
            {
                utf8JsonWriter.Reset(bufferWriter);
            }
            else
            {
                utf8JsonWriter.Dispose();
                utf8JsonWriter = new Utf8JsonWriter(bufferWriter, GetJsonWriterOptions(options));
                bufferWriterUtf8JsonWriter = utf8JsonWriter;
            }

            JsonSerializer.Serialize(utf8JsonWriter, obj, obj.GetType(), options);
        }

        private static bool OptionsEquals(JsonWriterOptions options1, JsonSerializerOptions options2)
        {
            return options1.Encoder == options2.Encoder && options1.Indented == options2.WriteIndented;
        }

        private static JsonWriterOptions GetJsonWriterOptions(JsonSerializerOptions options)
        {
            return new JsonWriterOptions
            {
                Encoder = options.Encoder,
                Indented = options.WriteIndented,
                SkipValidation = true,
            };
        }
    }
}