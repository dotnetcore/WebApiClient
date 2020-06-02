using System;
using System.Net.Http;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示服务上下文
    /// </summary>
    class ServiceContext
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
        /// 服务上下文
        /// </summary>
        /// <param name="client"></param>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ServiceContext(HttpClient client, IServiceProvider services, HttpApiOptions options)
        {
            this.Client = client ?? throw new ArgumentNullException(nameof(client));
            this.Services = services ?? throw new ArgumentNullException(nameof(services));
            this.Options = options ?? throw new ArgumentNullException(nameof(options));
        }
    }
}
