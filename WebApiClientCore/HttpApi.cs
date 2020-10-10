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
            return new HttpApiEmitActivator<THttpApi>().CreateInstance(actionInterceptor);
        }


        /// <summary>
        /// 表示httpApi方法调用的拦截器
        /// </summary>
        private class ActionInterceptor : IActionInterceptor
        {
            /// <summary>
            /// 服务上下文
            /// </summary>
            private readonly HttpClientContext context;

            /// <summary>
            /// httpApi方法调用的拦截器
            /// </summary>
            /// <param name="context">服务上下文</param> 
            public ActionInterceptor(HttpClientContext context)
            {
                this.context = context;
            }

            /// <summary>
            /// 拦截方法的调用
            /// </summary>
            /// <param name="actionInvoker">action执行器</param> 
            /// <param name="arguments">方法的参数集合</param>
            /// <returns></returns>
            public object Intercept(IActionInvoker actionInvoker, object?[] arguments)
            {
                return actionInvoker.Invoke(this.context, arguments);
            }
        }
    }
}
