using System;
using System.Net.Http;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示HttpClient上下文
    /// </summary>
    public class HttpClientContext
    {
        /// <summary>
        /// 获取HttpClient实例
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
        /// HttpClient上下文
        /// </summary>
        /// <param name="client">httpClient实例</param>
        /// <param name="services">服务提供者</param>
        /// <param name="options">Api配置选项</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpClientContext(HttpClient client, IServiceProvider services, HttpApiOptions options)
        {
            this.Client = client ?? throw new ArgumentNullException(nameof(client));
            this.Services = services ?? throw new ArgumentNullException(nameof(services));
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
        }
    }
}
