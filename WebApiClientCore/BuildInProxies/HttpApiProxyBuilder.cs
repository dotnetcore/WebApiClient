using System;
using System.Linq;
using System.Threading;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示THttpApi的代理类的实例创建者
    /// </summary>
    /// <typeparam name="THttpApi">接口类型</typeparam>
    class HttpApiProxyBuilder<THttpApi>
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
        /// IHttpApi的代理类的实例创建者
        /// </summary> 
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ProxyTypeCreateException"></exception>
        private HttpApiProxyBuilder()
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
        /// 创建IHttpApi的代理类的实例
        /// </summary>
        /// <param name="actionInterceptor">拦截器</param>
        /// <returns></returns>
        private THttpApi BuildCore(IActionInterceptor actionInterceptor)
        {
            return this.proxyTypeCtor.Invoke(actionInterceptor, this.actionInvokers);
        }


        /// <summary>
        /// 代理生成器的实例
        /// </summary>
        private static HttpApiProxyBuilder<THttpApi>? instance;

        /// <summary>
        /// 创建IHttpApi的代理类的实例
        /// </summary>
        /// <param name="actionInterceptor">拦截器</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ProxyTypeCreateException"></exception>
        /// <returns></returns>
        public static THttpApi Build(IActionInterceptor actionInterceptor)
        {
            var builder = Volatile.Read(ref instance);
            if (builder == null)
            {
                Interlocked.CompareExchange(ref instance, new HttpApiProxyBuilder<THttpApi>(), null);
                builder = instance;
            }

            return builder.BuildCore(actionInterceptor);
        }
    }
}
