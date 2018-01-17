using System.Net.Http;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示Delete请求
    /// 不可继承
    /// </summary>
    public sealed class HttpDeleteAttribute : HttpMethodAttribute
    {
        /// <summary>
        /// Delete请求
        /// </summary>
        public HttpDeleteAttribute()
            : base(HttpMethod.Delete)
        {
        }

        /// <summary>
        /// Delete请求
        /// </summary>
        /// <param name="path">请求绝对或相对路径</param>
        public HttpDeleteAttribute(string path)
            : base(HttpMethod.Delete, path)
        {
        }
    }
}
