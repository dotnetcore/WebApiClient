using System;
using System.Reflection;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示HttpApi代理描述
    /// 提供HttpApi代理类的实例化
    /// </summary>
    partial class HttpApiProxy
    {
        /// <summary>
        /// 代理类型的创建工厂
        /// </summary>
        private readonly Func<IActionInterceptor, MethodInfo[], IHttpApi> proxyFactory;

        /// <summary>
        /// 获取代理类型
        /// </summary>
        public Type ProxyType { get; }

        /// <summary>
        /// 获取接口声明的Api方法
        /// </summary>
        public MethodInfo[] ApiMethods { get; }

        /// <summary>
        /// 代理描述
        /// </summary>
        /// <param name="proxyType">代理类型</param>
        /// <param name="apiMethods">接口声明的api方法</param>
        /// <exception cref="ArgumentNullException"></exception>
        private HttpApiProxy(Type proxyType, MethodInfo[] apiMethods)
        {
            this.ProxyType = proxyType ?? throw new ArgumentNullException(nameof(proxyType));
            this.ApiMethods = apiMethods ?? throw new ArgumentNullException(nameof(apiMethods));
            this.proxyFactory = Lambda.CreateCtorFunc<IActionInterceptor, MethodInfo[], IHttpApi>(proxyType);
        }

        /// <summary>
        /// 创建新的HttpApi代理实例
        /// </summary>
        /// <param name="interceptor">拦截器</param>
        /// <returns></returns>
        private IHttpApi CreateInstance(IActionInterceptor interceptor)
        {
            return this.proxyFactory.Invoke(interceptor, this.ApiMethods);
        }
    }
}
