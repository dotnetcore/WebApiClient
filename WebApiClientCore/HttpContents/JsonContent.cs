using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using WebApiClientCore.Internals;
using WebApiClientCore.Serialization;

namespace WebApiClientCore.HttpContents
{
    /// <summary>
    /// 表示 uft8 的 json 内容
    /// </summary>
    public class JsonContent : BufferContent
    {
        private const string mediaType = "application/json";
        private static readonly MediaTypeHeaderValue defaultMediaType = new(mediaType);

        /// <summary>
        /// 获取对应的ContentType
        /// </summary>
        public static string MediaType => mediaType;

        /// <summary>
        /// uft8 的 json 内容
        /// </summary> 
        public JsonContent()
            : base(defaultMediaType)
        {
        }

        /// <summary>
        /// uft8 的 json 内容
        /// </summary>
        /// <param name="value">对象值</param>
        /// <param name="jsonSerializerOptions">json序列化选项</param> 
        [RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
        [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
        public JsonContent(object? value, JsonSerializerOptions? jsonSerializerOptions)
            : this(value, jsonSerializerOptions, null)
        {
        }

        /// <summary>
        /// json内容
        /// </summary>
        /// <param name="value">对象值</param>
        /// <param name="jsonSerializerOptions">json序列化选项</param>
        /// <param name="encoding">编码</param>
        [RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
        [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
        public JsonContent(object? value, JsonSerializerOptions? jsonSerializerOptions, Encoding? encoding)
        {
            if (encoding == null || Encoding.UTF8.Equals(encoding))
            {
                this.Headers.ContentType = defaultMediaType;
                JsonBufferSerializer.Serialize(this, value, jsonSerializerOptions);
            }
            else
            {
                this.Headers.ContentType = new MediaTypeHeaderValue(mediaType) { CharSet = encoding.WebName };
                using var utf8Writer = new RecyclableBufferWriter<byte>();
                JsonBufferSerializer.Serialize(utf8Writer, value, jsonSerializerOptions);
                Encoding.UTF8.Convert(encoding, utf8Writer.WrittenSpan, this);
            }
        }
    }
}
