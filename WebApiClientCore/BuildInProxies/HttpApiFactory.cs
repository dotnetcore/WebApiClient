using System;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示THttpApi的创建工厂
    /// </summary> 
    static class HttpApiFactory
    {
        /// <summary>
        /// 创建接口的实例
        /// </summary>
        /// <typeparam name="THttpApi"></typeparam>
        /// <param name="actionInterceptor">拦截器</param>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ProxyTypeCreateException"></exception>
        /// <returns></returns>
        public static THttpApi Create<THttpApi>(IActionInterceptor actionInterceptor)
        {
            return new HttpApiEmitActivator<THttpApi>().CreateInstance(actionInterceptor);
        }
    }
}