using System.Net.Http;
using System.Net.Http.Headers;

namespace WebApiClientCore.Benchmarks.Requests
{
    /// <summary>
    /// 表示http请求的json内容
    /// </summary>
    class ByteArrayJsonContent : ByteArrayContent
    { 
        /// <summary>
        /// http请求的json内容
        /// </summary>
        /// <param name="json">utf8 json</param>
        public ByteArrayJsonContent(byte[] json)
            : base(json)
        {
            this.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }
    }
}
