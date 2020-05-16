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
        private static readonly ConcurrentCache<MethodInfo, IActionInvoker> staticCache = new ConcurrentCache<MethodInfo, IActionInvoker>();

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
            var invoker = staticCache.GetOrAdd(method, CreateActionInvoker);
            return invoker.Invoke(this.context, arguments);
        }

        /// <summary>
        /// 创建action执行器
        /// </summary>
        /// <param name="method">接口的方法</param>
        /// <returns></returns>
        private static IActionInvoker CreateActionInvoker(MethodInfo method)
        {
            var apiAction = new ApiActionDescriptor(method);
            return new MultiplexedActionInvoker(apiAction);
        }
    }
}
