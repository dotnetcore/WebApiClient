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
        /// 表示patch请求方式
        /// </summary>
        private static readonly HttpMethod patchMethod = new HttpMethod("PATCH");

        /// <summary>
        /// Patch请求
        /// </summary>
        public HttpPatchAttribute()
            : base(patchMethod)
        {
        }

        /// <summary>
        /// Patch请求
        /// </summary>
        /// <param name="path">请求绝对或相对路径</param>
        public HttpPatchAttribute(string path)
            : base(patchMethod, path)
        {
        }
    }
}
