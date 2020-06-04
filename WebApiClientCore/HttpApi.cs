using System;
using System.Net.Http;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore
{
    /// <summary>
    /// 提供创建THttpApi的代理实例
    /// </summary>
    public static class HttpApi
    {
        /// <summary>
        /// 创建THttpApi的代理实例
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="client">httpClient</param>
        /// <param name="services">服务提供者</param>
        /// <param name="options">配置选项</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ProxyTypeCreateException"></exception>
        /// <returns></returns>
        public static THttpApi Create<THttpApi>(HttpClient client, IServiceProvider services, HttpApiOptions options)
        {
            var context = new ServiceContext(client, services, options);
            return Create<THttpApi>(context);
        }

        /// <summary>
        /// 创建THttpApi的代理实例
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="context">服务上下文</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ProxyTypeCreateException"></exception>
        /// <returns></returns>
        public static THttpApi Create<THttpApi>(ServiceContext context)
        {
            var interceptor = new ActionInterceptor(context);
            return HttpApiProxyBuilder<THttpApi>.Build(interceptor);
        }
    }
}
