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
        /// <param name="httpClient">httpClient</param>
        /// <param name="serviceProvider">服务提供者</param>
        /// <param name="httpApiOptions">Api配置选项</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ProxyTypeCreateException"></exception>
        /// <returns></returns>
        public static THttpApi Create<THttpApi>(HttpClient httpClient, IServiceProvider serviceProvider, HttpApiOptions httpApiOptions)
        {
            return Create<THttpApi>(new HttpClientContext(httpClient, serviceProvider, httpApiOptions));
        }

        /// <summary>
        /// 创建THttpApi的代理实例
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="httpClientContext">httpClient上下文</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ProxyTypeCreateException"></exception>
        /// <returns></returns>
        public static THttpApi Create<THttpApi>(HttpClientContext httpClientContext)
        {
            return Create<THttpApi>(new ActionInterceptor(httpClientContext));
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
