using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示httpApi方法调用的拦截器
    /// </summary>
    class ActionInterceptor : IActionInterceptor
    {
        /// <summary>
        /// action描述缓存
        /// </summary>
        private static readonly ConcurrentCache<MethodInfo, InvokerDescriptor> staticCache = new ConcurrentCache<MethodInfo, InvokerDescriptor>();

        /// <summary>
        /// 服务上下文
        /// </summary>
        private readonly ServiceContext context;

        /// <summary>
        /// httpApi方法调用的拦截器
        /// </summary>
        /// <param name="context">服务上下文</param> 
        public ActionInterceptor(ServiceContext context)
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
            var descriptor = staticCache.GetOrAdd(method, this.CreateInvokerDescriptor);
            if (descriptor.Action.Return.IsTaskDefinition == true)
            {
                return descriptor.ActionInvoker.Invoke(this.context, arguments);
            }
            else
            {
                return descriptor.ActionTaskCtor.Invoke(descriptor.Action, this.context, arguments);
            }
        }

        /// <summary>
        /// 创建执行者描述器
        /// </summary>
        /// <param name="method">接口的方法</param>
        /// <returns></returns>
        private InvokerDescriptor CreateInvokerDescriptor(MethodInfo method)
        {
            var apiAction = this.context.Services
                .GetRequiredService<IApiActionDescriptorProvider>()
                .CreateApiActionDescriptor(method);

            return new InvokerDescriptor(apiAction);
        }
    }
}
