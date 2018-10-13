using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WebApiClient.Contexts;

namespace WebApiClient.Attributes
{
    /// <summary>
    /// 表示http代理特性
    /// 接口实例整个生命周期内都使用这个代理
    /// </summary>    
    [DebuggerDisplay("Proxy {host}:{port}")]
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
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
            var handler = context.HttpApiConfig.HttpHandler;
            var proxyUsed = handler.UseProxy && handler.Proxy != null;

            if (proxyUsed == false)
            {
                handler.UseProxy = true;
                handler.Proxy = this.httpProxy;
            }
            else if (HttpProxy.IsProxyEquals(handler.Proxy, this.httpProxy) == false)
            {
                throw new HttpApiConfigException("不支持在请求之后切换代理设置");
            }
            return ApiTask.CompletedTask;
        }
    }
}
