using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示http代理特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    [DebuggerDisplay("Proxy {host}:{port}")]
    public class ProxyAttribute : ApiActionAttribute
    {
        /// <summary>
        /// http代理
        /// </summary>
        private readonly HttpProxy httpProxy;

        /// <summary>
        /// http代理描述
        /// </summary>
        /// <param name="host">域名或ip</param>
        /// <param name="port">端口</param>    
        /// <exception cref="ArgumentNullException"></exception>
        public ProxyAttribute(string host, int port)
        {
            this.httpProxy = new HttpProxy(host, port);
        }

        /// <summary>
        /// http代理描述
        /// </summary>
        /// <param name="host">域名或ip</param>
        /// <param name="port">端口</param>
        /// <param name="userName">账号</param>
        /// <param name="password">密码</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ProxyAttribute(string host, int port, string userName, string password)
        {
            this.httpProxy = new HttpProxy(host, port, userName, password);
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override Task BeforeRequestAsync(ApiActionContext context)
        {
            context.HttpApiConfig.HttpClient.SetProxy(this.httpProxy);
            return ApiTask.CompletedTask;
        }
    }
}
