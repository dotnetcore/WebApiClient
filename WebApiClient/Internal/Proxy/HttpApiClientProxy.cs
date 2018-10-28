using System;
using System.Reflection;

namespace WebApiClient
{
    /// <summary>
    /// 表示HttpApiClient代理描述
    /// 提供HttpApiClient代理类的实例化
    /// </summary>
    partial class HttpApiClientProxy
    {
        /// <summary>
        /// 代理类型的创建工厂
        /// </summary>
        private readonly Func<IApiInterceptor, MethodInfo[], HttpApiClient> proxyFactory;

        /// <summary>
        /// 获取代理类型
        /// </summary>
        public Type ProxyType { get; private set; }

        /// <summary>
        /// 获取接口声明的Api方法
        /// </summary>
        public MethodInfo[] ApiMethods { get; private set; }

        /// <summary>
        /// 代理描述
        /// </summary>
        /// <param name="proxyType">代理类型</param>
        /// <param name="apiMethods">接口声明的api方法</param>
        /// <exception cref="ArgumentNullException"></exception>
        private HttpApiClientProxy(Type proxyType, MethodInfo[] apiMethods)
        {
            this.ProxyType = proxyType ?? throw new ArgumentNullException(nameof(proxyType));
            this.ApiMethods = apiMethods ?? throw new ArgumentNullException(nameof(apiMethods));
            this.proxyFactory = Lambda.CreateNewFunc<HttpApiClient, IApiInterceptor, MethodInfo[]>(proxyType);
        }

        /// <summary>
        /// 创建新的HttpApiClient代理实例
        /// </summary>
        /// <param name="interceptor">拦截器</param>
        /// <returns></returns>
        private HttpApiClient CreateInstance(IApiInterceptor interceptor)
        {
            return this.proxyFactory.Invoke(interceptor, this.ApiMethods);
        }
    }
}
