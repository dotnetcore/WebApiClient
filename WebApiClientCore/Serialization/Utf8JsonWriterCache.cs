using System;
using System.Buffers;
using System.Text.Json;

namespace WebApiClientCore.Serialization
{
    /// <summary>
    /// Utf8JsonWriter缓存
    /// </summary>
    static class Utf8JsonWriterCache
    {
        [ThreadStatic]
        private static Utf8JsonWriter? threadUtf8JsonWriter;

        /// <summary>
        /// 获取与 bufferWriter 关联的 Utf8JsonWriter
        /// </summary>
        /// <param name="bufferWriter"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static Utf8JsonWriter Get(IBufferWriter<byte> bufferWriter, JsonSerializerOptions options)
        {
            var utf8JsonWriter = threadUtf8JsonWriter;
            if (utf8JsonWriter == null)
            {
                utf8JsonWriter = new Utf8JsonWriter(bufferWriter, GetJsonWriterOptions(options));
                threadUtf8JsonWriter = utf8JsonWriter;
            }
            else if (OptionsEquals(utf8JsonWriter.Options, options))
            {
                utf8JsonWriter.Reset(bufferWriter);
            }
            else
            {
                utf8JsonWriter.Dispose();
                utf8JsonWriter = new Utf8JsonWriter(bufferWriter, GetJsonWriterOptions(options));
                threadUtf8JsonWriter = utf8JsonWriter;
            }
            return utf8JsonWriter;
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
