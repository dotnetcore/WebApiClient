using System;
using System.Net.Http;

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
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public static THttpApi Create<THttpApi>(HttpClient client, IServiceProvider services, HttpApiOptions options)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            var context = new ServiceContext(client, services, options);
            var interceptor = new ActionInterceptor(context);
            return (THttpApi)Create(typeof(THttpApi), interceptor);
        }

        /// <summary>
        /// 创建IHttpApi的代理实例
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <param name="interceptor">拦截器</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        private static object Create(Type interfaceType, IActionInterceptor interceptor)
        {
            var builder = proxyBuilderCache.GetOrAdd(interfaceType, @interface => new HttpApiProxyBuilder(@interface));
            return builder.Build(interceptor);
        }
    }
}
