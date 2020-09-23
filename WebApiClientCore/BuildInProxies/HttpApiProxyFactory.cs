using System;
using System.Linq;
using System.Threading;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示THttpApi的代理类的创建工厂
    /// </summary>
    /// <typeparam name="THttpApi">接口类型</typeparam>
    class HttpApiProxyFactory<THttpApi>
    {
        /// <summary>
        /// 接口包含的所有action执行器 
        /// </summary>
        private readonly IActionInvoker[] actionInvokers;

        /// <summary>
        /// 接口代理类的构造器
        /// </summary>
        private readonly Func<IActionInterceptor, IActionInvoker[], THttpApi> proxyTypeCtor;

        /// <summary>
        /// 表示THttpApi的代理类的创建工厂
        /// </summary> 
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ProxyTypeCreateException"></exception>
        private HttpApiProxyFactory()
        {
            var interfaceType = typeof(THttpApi);

            this.actionInvokers = interfaceType
                .GetAllApiMethods()
                .Select(item => new ApiActionDescriptor(item, interfaceType))
                .Select(item => CreateActionInvoker(item))
                .ToArray();

            var proxyType = HttpApiProxyTypeBuilder.Build(interfaceType, this.actionInvokers);
            this.proxyTypeCtor = Lambda.CreateCtorFunc<IActionInterceptor, IActionInvoker[], THttpApi>(proxyType);

            static IActionInvoker CreateActionInvoker(ApiActionDescriptor apiAction)
            {
                var resultType = apiAction.Return.DataType.Type;
                var invokerType = typeof(MultiplexedActionInvoker<>).MakeGenericType(resultType);
                return invokerType.CreateInstance<IActionInvoker>(apiAction);
            }
        }

        /// <summary>
        /// 创建代理类的实例
        /// </summary>
        /// <param name="actionInterceptor">拦截器</param>
        /// <returns></returns>
        private THttpApi CreateProxy(IActionInterceptor actionInterceptor)
        {
            return this.proxyTypeCtor.Invoke(actionInterceptor, this.actionInvokers);
        }


        /// <summary>
        /// 代理创建工厂的实例
        /// </summary>
        private static HttpApiProxyFactory<THttpApi>? instance;

        /// <summary>
        /// 创建代理类的实例
        /// </summary>
        /// <param name="actionInterceptor">拦截器</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ProxyTypeCreateException"></exception>
        /// <returns></returns>
        public static THttpApi Create(IActionInterceptor actionInterceptor)
        {
            var fatory = Volatile.Read(ref instance);
            if (fatory == null)
            {
                Interlocked.CompareExchange(ref instance, new HttpApiProxyFactory<THttpApi>(), null);
                fatory = instance;
            }

            return fatory.CreateProxy(actionInterceptor);
        }
    }
}