using System.Net.Http;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示Get请求
    /// 不可继承
    /// </summary>
    public sealed class HttpGetAttribute : HttpMethodAttribute
    {
        /// <summary>
        /// Get请求
        /// </summary>
        public HttpGetAttribute()
            : base(HttpMethod.Get)
        {
        }

        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="path">请求绝对或相对路径</param>
        public HttpGetAttribute(string path)
            : base(HttpMethod.Get, path)
        {
        }
    }
}
