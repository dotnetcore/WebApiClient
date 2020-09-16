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
        /// 获取选项名称 
        /// </summary>
        public string OptionsName { get; }

        /// <summary>
        /// HttpClient上下文
        /// </summary>
        /// <param name="httpClient">httpClient实例</param>
        /// <param name="serviceProvider">服务提供者</param>
        /// <param name="httpApiOptions">Api配置选项</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpClientContext(HttpClient httpClient, IServiceProvider serviceProvider, HttpApiOptions httpApiOptions)
            : this(httpClient, serviceProvider, httpApiOptions, string.Empty)
        {
        }

        /// <summary>
        /// HttpClient上下文
        /// </summary>
        /// <param name="httpClient">httpClient实例</param>
        /// <param name="serviceProvider">服务提供者</param>
        /// <param name="httpApiOptions">Api配置选项</param>
        /// <param name="optionsName">选项名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        public HttpClientContext(HttpClient httpClient, IServiceProvider serviceProvider, HttpApiOptions httpApiOptions, string optionsName)
        {
            this.HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            this.HttpApiOptions = httpApiOptions ?? throw new ArgumentNullException(nameof(httpApiOptions));
            this.OptionsName = optionsName ?? string.Empty;
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (string.IsNullOrEmpty(this.OptionsName))
            {
                return base.ToString();
            }
            return this.OptionsName;
        }
    }
}
