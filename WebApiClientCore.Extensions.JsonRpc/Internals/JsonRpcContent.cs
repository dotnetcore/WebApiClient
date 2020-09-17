using WebApiClientCore.HttpContents;

namespace WebApiClientCore.Extensions.JsonRpc
{
    /// <summary>
    /// 表示JsonRpc请求内容
    /// </summary>
    class JsonRpcContent : BufferContent
    {
        /// <summary>
        /// 获取对应的ContentType
        /// </summary>
        public static string MediaType => "application/json-rpc";

        /// <summary>
        /// JsonRpc请求内容
        /// </summary> 
        /// <param name="mediaType">媒体类型</param>
        public JsonRpcContent(string? mediaType)
            : base(mediaType ?? MediaType)
        {
        }
    }
}
