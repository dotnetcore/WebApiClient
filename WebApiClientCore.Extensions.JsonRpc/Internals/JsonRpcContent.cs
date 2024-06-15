using System.Text.Json;
using WebApiClientCore.HttpContents;
using WebApiClientCore.Serialization;

namespace WebApiClientCore.Extensions.JsonRpc
{
    /// <summary>
    /// 表示JsonRpc请求内容
    /// </summary>
    sealed class JsonRpcContent : BufferContent
    {
        /// <summary>
        /// 获取对应的ContentType
        /// </summary>
        public static string MediaType => "application/json-rpc"; 

        /// <summary>
        /// uft8 的 json 内容
        /// </summary>
        /// <param name="mediaType"></param>
        /// <param name="value">对象值</param>
        /// <param name="jsonSerializerOptions">json序列化选项</param> 
        public JsonRpcContent(object? value, JsonSerializerOptions? jsonSerializerOptions, string? mediaType)
            : base(mediaType ?? MediaType)
        {
            JsonBufferSerializer.Serialize(this, value, jsonSerializerOptions);
        }
    }
}
