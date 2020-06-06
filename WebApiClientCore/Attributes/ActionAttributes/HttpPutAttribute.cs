using System.Net.Http;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示Put请求
    /// </summary>
    public class HttpPutAttribute : HttpMethodAttribute
    {
        /// <summary>
        /// Put请求
        /// </summary>
        public HttpPutAttribute()
           : this(path: null)
        {
        }

        /// <summary>
        /// Put请求
        /// </summary>
        /// <param name="path">请求绝对或相对路径</param>
        public HttpPutAttribute(string? path)
            : base(HttpMethod.Put, path)
        {
        }
    }
}
