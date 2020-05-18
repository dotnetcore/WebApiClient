using System.Net.Http;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示Patch请求
    /// 不可继承
    /// </summary>
    public sealed class HttpPatchAttribute : HttpMethodAttribute
    {
        /// <summary>
        /// Patch请求
        /// </summary>
        public HttpPatchAttribute()
            : base(HttpMethod.Patch)
        {
        }

        /// <summary>
        /// Patch请求
        /// </summary>
        /// <param name="path">请求绝对或相对路径</param>
        public HttpPatchAttribute(string path)
            : base(HttpMethod.Patch, path)
        {
        }
    }
}
