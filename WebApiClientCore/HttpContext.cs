using System;
using System.Net.Http;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示http上下文
    /// </summary>
    public class HttpContext : IDisposable
    {
        /// <summary>
        /// 获取关联的HttpClient实例
        /// </summary>
        public HttpClient Client { get; }

        /// <summary>
        /// 获取Api配置选项
        /// </summary>
        public HttpApiOptions Options { get; }

        /// <summary>
        /// 获取服务提供者
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        /// 获取关联的HttpRequestMessage
        /// </summary>
        public HttpApiRequestMessage RequestMessage { get; }

        /// <summary>
        /// 获取关联的的HttpResponseMessage
        /// </summary>
        public HttpResponseMessage ResponseMessage { get; internal set; }

        /// <summary>
        /// http上下文
        /// </summary>
        /// <param name="client">httpClient</param>
        /// <param name="services">服务提供者</param>
        /// <param name="options">接口选项</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpContext(HttpClient client, IServiceProvider services, HttpApiOptions options)
        {
            this.Client = client ?? throw new ArgumentNullException(nameof(client));
            this.Services = services ?? throw new ArgumentNullException(nameof(services));
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
            this.RequestMessage = new HttpApiRequestMessage { RequestUri = options.HttpHost ?? client.BaseAddress };
        }         

        /// <summary>
        /// 释放资源
        /// </summary> 
        void IDisposable.Dispose()
        {
            this.RequestMessage?.Dispose();
        }
    }
}
