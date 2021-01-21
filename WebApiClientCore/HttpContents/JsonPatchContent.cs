using System.Collections.Generic;
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
        /// <param name="oprations">patch操作项</param>
        /// <param name="jsonSerializerOptions">json序列化选项</param> 
        public JsonPatchContent(IEnumerable<object> oprations, JsonSerializerOptions? jsonSerializerOptions)
            : base(MediaType)
        {
            JsonBufferSerializer.Serialize(this, oprations, jsonSerializerOptions);
        }
    }
}
