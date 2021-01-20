using System;
using WebApiClientCore.Exceptions;
using WebApiClientCore.Implementations;
using WebApiClientCore.Internals.TypeProxies;

namespace WebApiClientCore
{
    /// <summary>
    /// 提供创建THttpApi的代理实例
    /// </summary>
    public static class HttpApi
    {
        /// <summary>
        /// 获取接口的别名
        /// 该别名可用于接口对应的OptionsName
        /// </summary>
        /// <param name="httpApiType">接口类型</param>
        /// <returns></returns>
        public static string GetName(Type? httpApiType)
        {
            if (httpApiType == null)
            {
                return string.Empty;
            }
            return httpApiType.FullName;
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
            var interceptor = new ActionInterceptor(httpClientContext);
            return Create<THttpApi>(ApiActionProvider.Default, interceptor);
        }

        /// <summary>
        /// 创建THttpApi的代理实例
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="actionProvider">Action提供者</param>  
        /// <param name="actionInterceptor">Action拦截器</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ProxyTypeCreateException"></exception>
        /// <returns></returns>
        public static THttpApi Create<THttpApi>(IApiActionProvider actionProvider, IActionInterceptor actionInterceptor)
        {
            var activator = new HttpApiEmitActivator<THttpApi>(actionProvider);
            return activator.CreateInstance(actionInterceptor);
        }
    }
}
