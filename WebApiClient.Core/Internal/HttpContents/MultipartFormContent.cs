using System;
using System.Net.Http;
using System.Net.Http.Headers;

namespace WebApiClient
{
    /// <summary>
    /// 表示form-data表单
    /// </summary>
    class MultipartFormContent : MultipartContent
    {
        /// <summary>
        /// 获取对应的ContentType
        /// </summary>
        public static string MediaType => "multipart/form-data";

        /// <summary>
        /// form-data表单
        /// </summary>
        public MultipartFormContent()
            : this(Guid.NewGuid().ToString())
        {
        }

        /// <summary>
        /// form-data表单
        /// </summary>
        /// <param name="boundary">分隔符</param>
        public MultipartFormContent(string boundary)
            : base("form-data", boundary)
        {
            var parameter = new NameValueHeaderValue("boundary", boundary);
            this.Headers.ContentType.Parameters.Clear();
            this.Headers.ContentType.Parameters.Add(parameter);
        }
    }
}
