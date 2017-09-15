using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient
{
    /// <summary>
    /// 表示将自身作为Http代理
    /// </summary>
    public class Proxy : WebProxy, IApiParameterable
    {
        /// <summary>
        /// 将自身作为Http代理
        /// </summary>
        public Proxy()
            : base()
        {
        }

        /// <summary>
        /// 将自身作为Http代理
        /// </summary>
        /// <param name="host">代理服务器域名或ip</param>
        /// <param name="port">代理服务器端口</param>
        public Proxy(string host, int port)
            : base(host, port)
        {
        }

        /// <summary>
        /// http请求之前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">特性关联的参数</param>
        /// <returns></returns>
        Task IApiParameterable.BeforeRequestAsync(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            context.HttpClientContext.HttpClientHandler.Proxy = this;
            return TaskExtend.CompletedTask;
        }
    }
}
