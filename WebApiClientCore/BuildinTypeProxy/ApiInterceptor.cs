using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示http接口调用的拦截器
    /// </summary>
    class ApiInterceptor : IApiInterceptor
    {
        /// <summary>
        /// action描述缓存
        /// </summary>
        private static readonly ConcurrentCache<MethodInfo, ActionCacheValue> staticCache = new ConcurrentCache<MethodInfo, ActionCacheValue>();

        /// <summary>
        /// 服务上下文
        /// </summary>
        private readonly ServiceContext context;

        /// <summary>
        /// http接口调用的拦截器
        /// </summary>
        /// <param name="context">服务上下文</param> 
        public ApiInterceptor(ServiceContext context)
        {
            this.context = context;
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
            var actionValue = staticCache.GetOrAdd(method, this.CreateActionCacheValue);
            if (actionValue.ActionDescriptor.Return.IsTaskDefinition == true)
            {
                return actionValue.ActionInvoker.Invoke(this.context, arguments);
            }
            else
            {
                return actionValue.ActionTaskFactory(actionValue.ActionDescriptor, this.context, arguments);
            }
        }

        /// <summary>
        /// 创建ApiAction缓存值
        /// </summary>
        /// <param name="method">接口的方法</param>
        /// <returns></returns>
        private ActionCacheValue CreateActionCacheValue(MethodInfo method)
        {
            var apiAction = this.context.Services
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
            /// ApiActionTask的创建委托
            /// 由于ApiActionTask缓存了ServiceContext，所以ActionCacheValue不能直接缓存ApiActionTask的实例
            /// 只能缓存ApiActionTask实例的创建委托
            /// </summary>
            public Func<ApiActionDescriptor, ServiceContext, object[], object> ActionTaskFactory { get; }

            /// <summary>
            /// Api缓存值
            /// </summary>
            /// <param name="descriptor">Api描述</param>
            public ActionCacheValue(ApiActionDescriptor descriptor)
            {
                this.ActionDescriptor = descriptor;

                // (ApiActionDescriptor apiAction)
                var invokerType = typeof(ApiActionInvoker<>).MakeGenericType(descriptor.Return.DataType.Type);
                this.ActionInvoker = Lambda.CreateCtorFunc<ApiActionDescriptor, IApiActionInvoker>(invokerType)(descriptor);

                // (ApiActionDescriptor apiAction, ServiceContext context, object[] arguments)
                var actionTaskType = typeof(ApiActionTask<>).MakeGenericType(descriptor.Return.DataType.Type);
                this.ActionTaskFactory = Lambda.CreateCtorFunc<ApiActionDescriptor, ServiceContext, object[], object>(actionTaskType);
            }
        }
    }
}
