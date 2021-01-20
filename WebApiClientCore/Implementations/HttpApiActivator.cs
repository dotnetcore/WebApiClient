using System;
using System.Linq;
using System.Reflection;

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
        private readonly ApiActionInvoker[] actionInvokers;

        /// <summary>
        /// 创建工厂
        /// </summary>
        private readonly Func<IActionInterceptor, ApiActionInvoker[], THttpApi> factory;

        /// <summary>
        /// 获取接口的所有方法
        /// </summary>
        protected MethodInfo[] ApiMethods { get; }

        /// <summary>
        /// THttpApi的实例创建器抽象
        /// </summary>
        /// <param name="apiActionDescriptorProvider"></param>
        /// <param name="actionInvokerProvider"></param>
        public HttpApiActivator(IApiActionDescriptorProvider apiActionDescriptorProvider, IApiActionInvokerProvider actionInvokerProvider)
        {
            var interfaceType = typeof(THttpApi);
            this.ApiMethods = HttpApi.FindApiMethods(interfaceType);
            this.actionInvokers = this.ApiMethods
                 .Select(item => apiActionDescriptorProvider.CreateApiActionDescriptor(item, interfaceType))
                 .Select(item => actionInvokerProvider.CreateActionInvoker(item))
                 .ToArray();

            // 最后一步创建工厂
            this.factory = this.CreateFactory();
        }

        /// <summary>
        /// 创建实例工厂
        /// </summary>
        /// <returns></returns>
        protected abstract Func<IActionInterceptor, ApiActionInvoker[], THttpApi> CreateFactory();

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
