using System;
using System.Net.Http;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示http上下文
    /// </summary>
    public class HttpContext : HttpClientContext, IDisposable
    {
        /// <summary>
        /// 获取关联的HttpRequestMessage
        /// </summary>
        public HttpApiRequestMessage RequestMessage { get; }

        /// <summary>
        /// 获取关联的的HttpResponseMessage
        /// </summary>
        public HttpResponseMessage? ResponseMessage { get; internal set; }

        /// <summary>
        /// http上下文
        /// </summary>
        /// <param name="context">服务上下文</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpContext(HttpClientContext context)
            : this(context.Client, context.Services, context.Options)
        {
        }

        /// <summary>
        /// http上下文
        /// </summary>
        /// <param name="client">httpClient实例</param>
        /// <param name="services">服务提供者</param>
        /// <param name="options">httpApi选项</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpContext(HttpClient client, IServiceProvider services, HttpApiOptions options)
            : base(client, services, options)
        {
            this.RequestMessage = new HttpApiRequestMessage(options.HttpHost ?? client.BaseAddress);
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public virtual void Dispose()
        {
            this.RequestMessage?.Dispose();
        }
    }
}
