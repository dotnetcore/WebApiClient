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
        /// 代理实例创建者的缓存
        /// </summary>
        private static readonly ConcurrentCache<Type, HttpApiProxyBuilder> proxyBuilderCache = new ConcurrentCache<Type, HttpApiProxyBuilder>();

        /// <summary>
        /// 创建THttpApi的代理实例
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="client">httpClient</param>
        /// <param name="services">服务提供者</param>
        /// <param name="options">配置选项</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ProxyTypeCreateException"></exception>
        /// <returns></returns>
        public static THttpApi Create<THttpApi>(HttpClient client, IServiceProvider services, HttpApiOptions options)
        {
            return (THttpApi)Create(client, services, options, typeof(THttpApi));
        }

        /// <summary>
        /// 创建interfaceType的代理实例
        /// </summary>
        /// <param name="client">httpClient</param>
        /// <param name="services">服务提供者</param>
        /// <param name="options">配置选项</param>
        /// <param name="interfaceType">接口类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ProxyTypeCreateException"></exception>
        /// <returns></returns>
        public static object Create(HttpClient client, IServiceProvider services, HttpApiOptions options, Type interfaceType)
        {
            var context = new ServiceContext(client, services, options);
            var interceptor = new ActionInterceptor(context);

            var builder = proxyBuilderCache.GetOrAdd(interfaceType, @interface => new HttpApiProxyBuilder(@interface));
            return builder.Build(interceptor);
        }
    }
}
