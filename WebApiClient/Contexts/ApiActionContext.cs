using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示请求Api的上下文
    /// </summary>
    public class ApiActionContext
    {
        /// <summary>
        /// 获取关联的HttpApiClientConfig
        /// </summary>
        public HttpApiClientConfig HttpApiClientConfig { get; internal set; }

        /// <summary>
        /// 获取关联的HttpHostAttribute
        /// </summary>
        public HttpHostAttribute HostAttribute { get; internal set; }

        /// <summary>
        /// 获取关联的ApiReturnAttribute
        /// </summary>
        public ApiReturnAttribute ApiReturnAttribute { get; internal set; }

        /// <summary>
        /// 获取ApiActionFilterAttribute
        /// </summary>
        public ApiActionFilterAttribute[] ApiActionFilterAttributes { get; internal set; }

        /// <summary>
        /// 获取关联的ApiActionDescriptor
        /// </summary>
        public ApiActionDescriptor ApiActionDescriptor { get; internal set; }

        /// <summary>
        /// 获取关联的HttpRequestMessage
        /// </summary>
        public HttpRequestMessage RequestMessage { get; internal set; }

        /// <summary>
        /// 获取关联的HttpResponseMessage
        /// </summary>
        public HttpResponseMessage ResponseMessage { get; internal set; }
    }
}
