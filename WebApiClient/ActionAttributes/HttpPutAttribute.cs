using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示Put请求
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class HttpPutAttribute : HttpMethodAttribute
    {
        /// <summary>
        /// Put请求
        /// </summary>
        /// <param name="path">相对路径</param>
        public HttpPutAttribute(string path)
            : base(HttpMethod.Put, path)
        {
        }
    }
}
