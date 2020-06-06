using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace WebApiClientCore.Attributes
{
    /// <summary>
    /// 表示请求服务http绝对完整主机域名
    /// 例如http://www.abc.com/或http://www.abc.com/path/
    /// </summary>
    [DebuggerDisplay("Host = {Host}")]
    public class HttpHostAttribute : ApiActionAttribute
    {
        /// <summary>
        /// 获取完整主机域名
        /// </summary>
        public Uri Host { get; }

        /// <summary>
        /// 请求服务的根路径      
        /// </summary>
        /// <param name="host">例如http://www.abc.com/或http://www.abc.com/path/</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        public HttpHostAttribute(string host)
        {
            this.Host = new Uri(host, UriKind.Absolute);
            this.OrderIndex = int.MinValue;
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
