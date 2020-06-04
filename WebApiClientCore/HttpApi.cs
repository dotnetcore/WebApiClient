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
            var context = new HttpClientContext(client, services, options);
            return Create<THttpApi>(new ActionInterceptor(context));
        }

        /// <summary>
        /// 创建THttpApi的代理实例
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="actionInterceptor">Action拦截器</param>  
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ProxyTypeCreateException"></exception>
        /// <returns></returns>
        public static THttpApi Create<THttpApi>(IActionInterceptor actionInterceptor)
        {
            return HttpApiProxyBuilder<THttpApi>.Build(actionInterceptor);
        }
    }
}
