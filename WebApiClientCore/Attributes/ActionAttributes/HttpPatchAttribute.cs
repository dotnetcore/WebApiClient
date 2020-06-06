using System.Net.Http;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示Patch请求
    /// </summary>
    public class HttpPatchAttribute : HttpMethodAttribute
    {
        /// <summary>
        /// Patch请求
        /// </summary>
        public HttpPatchAttribute()
            : this(path: null)
        {
        }

        /// <summary>
        /// Patch请求
        /// </summary>
        /// <param name="path">请求绝对或相对路径</param>
        public HttpPatchAttribute(string? path)
            : base(HttpMethod.Patch, path)
        {
        }
    }
}
