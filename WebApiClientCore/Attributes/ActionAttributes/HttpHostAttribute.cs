using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示请求服务http绝对完整主机域名
    /// 例如http://www.abc.com/
    /// </summary>
    [DebuggerDisplay("Host = {Host}")]
    public partial class HttpHostAttribute : HttpHostBaseAttribute
    {
        /// <summary>
        /// 获取完整主机域名
        /// </summary>
        public Uri? Host { get; }

        /// <summary>
        /// 请求服务http绝对完整主机域名      
        /// </summary>
        /// <param name="host">例如http://www.abc.com</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        [AttributeCtorUsage(AttributeTargets.Interface | AttributeTargets.Method)]
        public HttpHostAttribute(string host)
        {
            this.Host = new Uri(host, UriKind.Absolute);
        }

        /// <summary>
        /// 执行前
        /// </summary>
        /// <param name="context">上下文</param>
        /// <returns></returns>
        public override Task OnRequestAsync(ApiRequestContext context)
        {
            if (context.HttpContext.RequestMessage.RequestUri == null)
            {
                context.HttpContext.RequestMessage.RequestUri = this.Host;
            }
            return Task.CompletedTask;
        }
    }
}
