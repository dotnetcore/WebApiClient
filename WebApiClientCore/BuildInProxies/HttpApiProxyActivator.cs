using System;
using System.Linq;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示THttpApi的代理类的实例创建器抽象
    /// </summary>
    /// <typeparam name="THttpApi"></typeparam>
    abstract class HttpApiProxyActivator<THttpApi>
    {
        /// <summary>
        /// 实例工厂
        /// </summary>
        private readonly Func<IActionInterceptor, THttpApi> factory;

        /// <summary>
        /// THttpApi的代理类的实例创建器
        /// </summary>
        public HttpApiProxyActivator()
        {
            this.factory = this.CreateFactory();
        }

        /// <summary>
        /// 创建代理类的实例工厂
        /// </summary>
        /// <returns></returns>
        protected abstract Func<IActionInterceptor, THttpApi> CreateFactory();

        /// <summary>
        /// 实例代理类的实例
        /// </summary>
        /// <param name="interceptor">拦截器</param>
        /// <returns></returns>
        public THttpApi CreateInstance(IActionInterceptor interceptor)
        {
            return this.factory(interceptor);
        }

        /// <summary>
        /// 返回接口的action执行器
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        protected IActionInvoker[] GetActionInvokers()
        {
            var interfaceType = typeof(THttpApi);
            return interfaceType
                .GetAllApiMethods()
                .Select(item => new ApiActionDescriptor(item, interfaceType))
                .Select(item => CreateActionInvoker(item))
                .ToArray();
        }

        /// <summary>
        /// 创建Action执行器
        /// </summary>
        /// <param name="apiAction">action描述</param>
        /// <returns></returns>
        private static IActionInvoker CreateActionInvoker(ApiActionDescriptor apiAction)
        {
            var resultType = apiAction.Return.DataType.Type;
            var invokerType = typeof(MultiplexedActionInvoker<>).MakeGenericType(resultType);
            return invokerType.CreateInstance<IActionInvoker>(apiAction);
        }
    }
}
