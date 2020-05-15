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
        private static readonly ConcurrentCache<MethodInfo, ActionCacheValue> cache = new ConcurrentCache<MethodInfo, ActionCacheValue>();

        private readonly HttpClient client;
        private readonly HttpApiOptions options;
        private readonly IServiceProvider services;


        /// <summary>
        /// http接口调用的拦截器
        /// </summary>
        /// <param name="client"></param>
        /// <param name="options"></param>
        /// <param name="services"></param>
        public ApiInterceptor(HttpClient client, HttpApiOptions options, IServiceProvider services)
        {
            this.client = client;
            this.options = options;
            this.services = services;
        }

        /// <summary>
        /// 拦截方法的调用
        /// </summary>
        /// <param name="target">接口的实例</param>
        /// <param name="method">接口的方法</param>
        /// <param name="arguments">接口的参数集合</param>
        /// <returns></returns>
        public object Intercept(object target, MethodInfo method, object[] arguments)
        {
            var actionValue = cache.GetOrAdd(method, this.CreateActionCacheValue);
            using var httpContext = new HttpContext(this.client, this.services, this.options);

            var context = new ApiRequestContext(httpContext, actionValue.ActionDescriptor, arguments);
            return actionValue.ActionInvoker.InvokeAsync(context);
        }

        /// <summary>
        /// 创建ApiAction缓存值
        /// </summary>
        /// <param name="method">接口的方法</param>
        /// <returns></returns>
        private ActionCacheValue CreateActionCacheValue(MethodInfo method)
        {
            var apiAction = this.services
                .GetRequiredService<IApiActionDescriptorProvider>()
                .CreateApiActionDescriptor(method);

            return new ActionCacheValue(apiAction);
        }

        /// <summary>
        /// 表示ApiAction缓存值
        /// </summary>
        private class ActionCacheValue
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
            public ActionCacheValue(ApiActionDescriptor descriptor)
            {
                this.ActionDescriptor = descriptor;
                var invokerType = typeof(ApiActionInvoker<>).MakeGenericType(descriptor.Return.DataType.Type);
                this.ActionInvoker = Lambda.CreateCtorFunc<ApiActionDescriptor, IApiActionInvoker>(invokerType)(descriptor);
            }
        }
    }
}
