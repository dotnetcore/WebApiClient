using System;
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
        /// 代理实例创建者
        /// </summary>
        private static HttpApiProxyActivator<THttpApi>? _activator;

        /// <summary>
        /// 创建代理类的实例
        /// </summary>
        /// <param name="actionInterceptor">拦截器</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ProxyTypeCreateException"></exception>
        /// <returns></returns>
        public static THttpApi Create(IActionInterceptor actionInterceptor)
        {
            var activator = Volatile.Read(ref _activator);
            if (activator == null)
            {
                Interlocked.CompareExchange(ref _activator, new HttpApiProxyEmitActivator<THttpApi>(), null);
                activator = _activator;
            }

            return activator.CreateInstance(actionInterceptor);
        }
    }
}