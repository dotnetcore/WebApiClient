using System.Net.Http;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示Options请求
    /// 不可继承
    /// </summary>
    public sealed class HttpOptionsAttribute : HttpMethodAttribute
    {
        /// <summary>
        /// Options请求
        /// </summary>
        public HttpOptionsAttribute()
            : base(HttpMethod.Options)
        {
        }

        /// <summary>
        /// Options请求
        /// </summary>
        /// <param name="path">相对路径</param>
        public HttpOptionsAttribute(string path)
            : base(HttpMethod.Options, path)
        {
        }
    }
}
