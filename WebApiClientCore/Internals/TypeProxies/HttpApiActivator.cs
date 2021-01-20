using System;
using System.Linq;
using System.Threading;

namespace WebApiClientCore.Internals.TypeProxies
{
    /// <summary>
    /// 表示THttpApi的实例创建器抽象
    /// </summary>
    /// <typeparam name="THttpApi"></typeparam>
    abstract class HttpApiActivator<THttpApi>
    {
        /// <summary>
        /// 实例工厂
        /// </summary>
        private static Func<IActionInterceptor, THttpApi>? _factory;

        /// <summary>
        /// Action执行器提供者
        /// </summary>
        private readonly IApiActionProvider actionProvider;

        /// <summary>
        /// THttpApi的实例创建器抽象
        /// </summary>
        /// <param name="actionProvider">Action提供者</param>
        public HttpApiActivator(IApiActionProvider actionProvider)
        {
            this.actionProvider = actionProvider;
        }

        /// <summary>
        /// 创建接口的实例
        /// </summary>
        /// <param name="interceptor">拦截器</param>
        /// <returns></returns>
        public THttpApi CreateInstance(IActionInterceptor interceptor)
        {
            var factory = Volatile.Read(ref _factory);
            if (factory == null)
            {
                Interlocked.CompareExchange(ref _factory, this.CreateFactory(), null);
                factory = _factory;
            }
            return factory(interceptor);
        }

        /// <summary>
        /// 创建实例工厂
        /// </summary>
        /// <returns></returns>
        protected abstract Func<IActionInterceptor, THttpApi> CreateFactory();

        /// <summary>
        /// 创建接口的action执行器
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        protected IActionInvoker[] CreateActionInvokers()
        {
            var interfaceType = typeof(THttpApi);
            return interfaceType
                .GetAllApiMethods()
                .Select(item => this.actionProvider.CreateDescriptor(item, interfaceType))
                .Select(item => this.actionProvider.CreateInvoker(item))
                .ToArray();
        }
    }
}
