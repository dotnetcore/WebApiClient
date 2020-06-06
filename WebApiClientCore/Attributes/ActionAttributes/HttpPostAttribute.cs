using System.Net.Http;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示Post请求
    /// </summary>
    public class HttpPostAttribute : HttpMethodAttribute
    {
        /// <summary>
        /// Post请求
        /// </summary>
        public HttpPostAttribute()
          : this(path: null)
        {
        }

        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="path">请求绝对或相对路径</param>
        public HttpPostAttribute(string? path)
            : base(HttpMethod.Post, path)
        {
        }
    }
}
