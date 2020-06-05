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
        public HttpClient HttpClient { get; }

        /// <summary>
        /// 获取Api配置选项
        /// </summary>
        public HttpApiOptions HttpApiOptions { get; }

        /// <summary>
        /// 获取服务提供者
        /// </summary>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// HttpClient上下文
        /// </summary>
        /// <param name="httpClient">httpClient实例</param>
        /// <param name="serviceProvider">服务提供者</param>
        /// <param name="httpApiOptions">Api配置选项</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpClientContext(HttpClient httpClient, IServiceProvider serviceProvider, HttpApiOptions httpApiOptions)
        {
            this.HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.HttpApiOptions = httpApiOptions ?? throw new ArgumentNullException(nameof(httpApiOptions));
        }
    }
}
