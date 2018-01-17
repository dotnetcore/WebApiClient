using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
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
        /// 域名或ip
        /// </summary>
        private readonly string host;

        /// <summary>
        /// 端口
        /// </summary>
        private readonly int port;

        /// <summary>
        /// 凭证
        /// </summary>
        private readonly ICredentials credential;

        /// <summary>
        /// http代理描述
        /// </summary>
        /// <param name="host">域名或ip</param>
        /// <param name="port">端口</param>    
        /// <exception cref="ArgumentNullException"></exception>
        public ProxyAttribute(string host, int port)
            : this(host, port, null, null)
        {
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
            if (string.IsNullOrEmpty(host))
            {
                throw new ArgumentNullException(nameof(host));
            }

            this.host = host;
            this.port = port;

            if (string.IsNullOrEmpty(userName) == false && string.IsNullOrEmpty(password) == false)
            {
                this.credential = new NetworkCredential(userName, password);
            }
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override Task BeforeRequestAsync(ApiActionContext context)
        {
            var proxy = new WebProxy(this.host, this.port)
            {
                Credentials = this.credential
            };
            context.HttpApiConfig.HttpClient.SetProxy(proxy);
            return ApiTask.CompletedTask;
        }
    }
}
