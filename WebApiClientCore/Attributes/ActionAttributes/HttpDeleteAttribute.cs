using System.Net.Http;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示Delete请求
    /// </summary>
    public class HttpDeleteAttribute : HttpMethodAttribute
    {
        /// <summary>
        /// Delete请求
        /// </summary>
        public HttpDeleteAttribute()
            : this(path: null)
        {
        }

        /// <summary>
        /// Delete请求
        /// </summary>
        /// <param name="path">请求绝对或相对路径</param>
        public HttpDeleteAttribute(string? path)
            : base(HttpMethod.Delete, path)
        {
        }
    }
}
