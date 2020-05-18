using System.Net.Http;
using System.Net.Http.Headers;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示http请求的json内容
    /// </summary>
    class JsonContent : ByteArrayContent
    {
        /// <summary>
        /// 获取对应的ContentType
        /// </summary>
        public static string MediaType => "application/json";

        /// <summary>
        /// http请求的json内容
        /// </summary>
        /// <param name="json">json内容</param>
        public JsonContent(byte[] json)
            : base(json)
        {
            this.Headers.ContentType = new MediaTypeHeaderValue(MediaType);
        }
    }
}
