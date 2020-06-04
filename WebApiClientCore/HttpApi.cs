using System;
using System.Net.Http;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore
{
    /// <summary>
    /// 提供创建THttpApi的代理实例
    /// </summary>
    public static class HttpApi<THttpApi>
    {
        /// <summary>
        /// 代理实例创建者的缓存
        /// </summary>
        private static readonly HttpApiProxyBuilder proxyBuilder = new HttpApiProxyBuilder(typeof(THttpApi));

        /// <summary>
        /// 创建THttpApi的代理实例
        /// </summary> 
        /// <param name="client">httpClient</param>
        /// <param name="services">服务提供者</param>
        /// <param name="options">配置选项</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ProxyTypeCreateException"></exception>
        /// <returns></returns>
        public static THttpApi Create(HttpClient client, IServiceProvider services, HttpApiOptions options)
        {
            var context = new ServiceContext(client, services, options);
            var interceptor = new ActionInterceptor(context);
            return (THttpApi)proxyBuilder.Build(interceptor);
        }
    }
}
