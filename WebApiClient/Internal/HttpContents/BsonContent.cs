using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WebApiClient
{
    /// <summary>
    /// 表示http请求的bson内容
    /// </summary>
    class BsonContent : StreamContent
    {
        /// <summary>
        /// 获取对应的ContentType
        /// </summary>
        public static string MediaType => "application/bson";

        /// <summary>
        /// http请求的bson内容
        /// </summary>
        /// <param name="bson">bson内容</param>
        public BsonContent(Stream bson)
            : base(bson)
        {
            this.Headers.ContentType = new MediaTypeHeaderValue(MediaType);
        }
    }
}
