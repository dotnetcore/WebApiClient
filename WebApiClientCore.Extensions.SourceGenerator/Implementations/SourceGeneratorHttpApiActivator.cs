using System;
using System.Reflection;
using WebApiClientCore.Exceptions;
using WebApiClientCore.Internals;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// 表示THttpApi的实例创建器
    /// 通过查找类型代理类型创建实例
    /// </summary>
    /// <typeparam name="THttpApi"></typeparam>
    public class SourceGeneratorHttpApiActivator<THttpApi> : HttpApiActivator<THttpApi>
    {
        /// <summary>
        /// 通过查找类型代理类型创建实例
        /// </summary>
        /// <param name="apiActionDescriptorProvider"></param>
        /// <param name="actionInvokerProvider"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public SourceGeneratorHttpApiActivator(IApiActionDescriptorProvider apiActionDescriptorProvider, IApiActionInvokerProvider actionInvokerProvider)
            : base(apiActionDescriptorProvider, actionInvokerProvider)
        {
        }

        /// <summary>
        /// 创建实例工厂
        /// </summary>
        /// <exception cref="ProxyTypeCreateException"></exception>
        /// <returns></returns>
        protected override Func<IHttpApiInterceptor, ApiActionInvoker[], THttpApi> CreateFactory()
        {
            var proxyType = FindProxyType(typeof(THttpApi));
            if (proxyType != null)
            {
                return LambdaUtil.CreateCtorFunc<IHttpApiInterceptor, ApiActionInvoker[], THttpApi>(proxyType);
            }

            var message = $"找不到{typeof(THttpApi)}的代理类";
            throw new ProxyTypeCreateException(typeof(THttpApi), message);
        }

        /// <summary>
        /// 从接口所在程序集查找代理类
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <returns></returns>
        private static Type? FindProxyType(Type interfaceType)
        {
            var httpApiType = interfaceType.IsGenericType
                ? interfaceType.GetGenericTypeDefinition()
                : interfaceType;

            foreach (var proxyType in interfaceType.Assembly.GetTypes())
            {
                if (proxyType.IsVisible || proxyType.IsClass == false)
                {
                    continue;
                }

                var proxyClassAttr = proxyType.GetCustomAttribute<HttpApiProxyClassAttribute>();
                if (proxyClassAttr == null || proxyClassAttr.HttpApiType != httpApiType)
                {
                    continue;
                }

                return proxyType.IsGenericType
                    ? proxyType.MakeGenericType(interfaceType.GetGenericArguments())
                    : proxyType;
            }

            return null;
        }
    }
}
