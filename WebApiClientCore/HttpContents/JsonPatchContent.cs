using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using WebApiClientCore.Serialization;

namespace WebApiClientCore.HttpContents
{
    /// <summary>
    /// 表示utf8的JsonPatch内容
    /// </summary>
    public class JsonPatchContent : BufferContent
    {
        /// <summary>
        /// 获取对应的ContentType
        /// </summary>
        public static string MediaType => "application/json-patch+json";

        /// <summary>
        /// utf8的JsonPatch内容
        /// </summary>
        public JsonPatchContent()
            : base(MediaType)
        {
        }

        /// <summary>
        /// utf8的JsonPatch内容
        /// </summary>
        /// <param name="operations">patch操作项</param>
        /// <param name="jsonSerializerOptions">json序列化选项</param> 
        [RequiresDynamicCode("JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.")]
        [RequiresUnreferencedCode("JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.")]
        public JsonPatchContent(IEnumerable<object> operations, JsonSerializerOptions? jsonSerializerOptions)
            : base(MediaType)
        {
            JsonBufferSerializer.Serialize(this, operations, jsonSerializerOptions);
        }
    }
}
