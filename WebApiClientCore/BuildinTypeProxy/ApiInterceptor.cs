using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Reflection;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示http接口调用的拦截器
    /// </summary>
    class ApiInterceptor : IApiInterceptor
    {
        /// <summary>
        /// ApiAction缓存
        /// </summary>
        private static readonly ConcurrentCache<MethodInfo, ApiCacheValue> cache = new ConcurrentCache<MethodInfo, ApiCacheValue>();

        private readonly HttpClient httpClient;
        private readonly HttpApiOptions apiOptions;
        private readonly IServiceProvider requestServices;


        /// <summary>
        /// http接口调用的拦截器
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="options"></param>
        /// <param name="requestServices"></param>
        public ApiInterceptor(HttpClient httpClient, HttpApiOptions options, IServiceProvider requestServices)
        {
            this.httpClient = httpClient;
            this.apiOptions = options;
            this.requestServices = requestServices;
        }

        /// <summary>
        /// 拦截方法的调用
        /// </summary>
        /// <param name="target">接口的实例</param>
        /// <param name="method">接口的方法</param>
        /// <param name="parameters">接口的参数集合</param>
        /// <returns></returns>
        public object Intercept(object target, MethodInfo method, object[] parameters)
        {
            var apiCache = cache.GetOrAdd(method, this.CreateApiCacheValue);
            using var httpContext = new HttpContext(this.httpClient, this.requestServices, this.apiOptions);
            var context = new ApiActionContext(httpContext, apiCache.ActionDescriptor, parameters);
            return apiCache.ActionInvoker.InvokeAsync(context);
        }

        /// <summary>
        /// 创建Api缓存值
        /// </summary>
        /// <param name="method">接口的方法</param>
        /// <returns></returns>
        private ApiCacheValue CreateApiCacheValue(MethodInfo method)
        {
            var apiAction = this.requestServices
                .GetRequiredService<IApiActionDescriptorProvider>()
                .CreateApiActionDescriptor(method);

            return new ApiCacheValue(apiAction);
        }

        /// <summary>
        /// 表示Api缓存值
        /// </summary>
        private class ApiCacheValue
        {
            /// <summary>
            /// Api描述
            /// </summary>
            public ApiActionDescriptor ActionDescriptor { get; }

            /// <summary>
            /// Api执行器
            /// </summary>
            public IApiActionInvoker ActionInvoker { get; }

            /// <summary>
            /// Api缓存值
            /// </summary>
            /// <param name="descriptor">Api描述</param>
            public ApiCacheValue(ApiActionDescriptor descriptor)
            {
                this.ActionDescriptor = descriptor;
                this.ActionInvoker = Lambda.CreateCtorFunc<IApiActionInvoker>(typeof(ApiActionInvoker<>).MakeGenericType(descriptor.Return.DataType.Type))();
            }
        }
    }
}
