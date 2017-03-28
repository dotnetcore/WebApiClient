using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示Post请求
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class HttpPostAttribute : HttpMethodAttribute
    {
        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="path">相对路径</param>
        public HttpPostAttribute(string path)
            : base(HttpMethod.Post, path)
        {
        }
    }
}
