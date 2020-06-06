using System.Net.Http;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示Get请求
    /// </summary>
    public class HttpGetAttribute : HttpMethodAttribute
    {
        /// <summary>
        /// Get请求
        /// </summary>
        public HttpGetAttribute()
           : this(path: null)
        {
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="path">请求绝对或相对路径</param>
        public HttpGetAttribute(string? path)
            : base(HttpMethod.Get, path)
        {
        }
    }
}
