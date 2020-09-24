using System;
using System.Threading;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示THttpApi的创建工厂
    /// </summary>
    /// <typeparam name="THttpApi">接口类型</typeparam>
    static class HttpApiFactory<THttpApi>
    {
        /// <summary>
        /// 实例创建者
        /// </summary>
        private static HttpApiActivator<THttpApi>? _activator;

        /// <summary>
        /// 创建接口的实例
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
                Interlocked.CompareExchange(ref _activator, new HttpApiEmitActivator<THttpApi>(), null);
                activator = _activator;
            }

            return activator.CreateInstance(actionInterceptor);
        }
    }
}