using System.Net.Http;
using System.Text;

namespace WebApiClient
{
    /// <summary>
    /// 表示http请求的JsonPatch内容
    /// </summary>
    class JsonPatchContent : StringContent
    {
        /// <summary>
        /// 获取对应的ContentType
        /// </summary>
        public static string MediaType => "application/json-patch+json";

        /// <summary>
        /// http请求的JsonPatch内容
        /// </summary>
        /// <param name="json">json内容</param>
        /// <param name="encoding">编码</param>
        public JsonPatchContent(string json, Encoding encoding)
            : base(json ?? string.Empty, encoding, MediaType)
        {
        }
    }
}
