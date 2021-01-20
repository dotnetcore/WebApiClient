using System;
using System.Linq;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// 表示THttpApi的实例创建器抽象
    /// </summary>
    /// <typeparam name="THttpApi"></typeparam>
    abstract class HttpApiActivator<THttpApi> : IHttpApiActivator<THttpApi>
    {
        /// <summary>
        /// 接口的所有方法执行器
        /// </summary>
        private readonly IActionInvoker[] actionInvokers;

        /// <summary>
        /// 创建工厂
        /// </summary>
        private readonly Func<IActionInterceptor, IActionInvoker[], THttpApi> factory;

        /// <summary>
        /// THttpApi的实例创建器抽象
        /// </summary>
        /// <param name="apiActionDescriptorProvider"></param>
        /// <param name="actionInvokerProvider"></param>
        public HttpApiActivator(IApiActionDescriptorProvider apiActionDescriptorProvider, IActionInvokerProvider actionInvokerProvider)
        {
            var interfaceType = typeof(THttpApi);
            this.actionInvokers = HttpApi.FindApiMethods(interfaceType)
                 .Select(item => apiActionDescriptorProvider.CreateApiActionDescriptor(item, interfaceType))
                 .Select(item => actionInvokerProvider.CreateActionInvoker(item))
                 .ToArray();

            this.factory = this.CreateFactory(this.actionInvokers);
        }

        /// <summary>
        /// 创建实例工厂
        /// </summary>
        /// <returns></returns>
        protected abstract Func<IActionInterceptor, IActionInvoker[], THttpApi> CreateFactory(IActionInvoker[] actionInvokers);

        /// <summary>
        /// 创建接口的实例
        /// </summary>
        /// <param name="interceptor">拦截器</param>
        /// <returns></returns>
        public THttpApi CreateInstance(IActionInterceptor interceptor)
        {
            return this.factory(interceptor, this.actionInvokers);
        }
    }
}
