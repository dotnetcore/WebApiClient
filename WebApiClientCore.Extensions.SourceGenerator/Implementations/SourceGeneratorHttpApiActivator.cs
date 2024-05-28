using System;
using System.Linq;
using System.Reflection;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// 表示THttpApi的实例创建器
    /// 通过查找类型代理类型创建实例
    /// </summary>
    /// <typeparam name="THttpApi"></typeparam>
    public class SourceGeneratorHttpApiActivator<
#if NET5_0_OR_GREATER
        [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.All)]
#endif
        THttpApi> : IHttpApiActivator<THttpApi>
    {
        private readonly ApiActionInvoker[] actionInvokers;
        private readonly Func<IHttpApiInterceptor, ApiActionInvoker[], THttpApi> activator;

        /// <summary>
        /// 通过查找类型代理类型创建实例
        /// </summary>
        /// <param name="apiActionDescriptorProvider"></param>
        /// <param name="actionInvokerProvider"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="ProxyTypeCreateException"></exception>
        public SourceGeneratorHttpApiActivator(IApiActionDescriptorProvider apiActionDescriptorProvider, IApiActionInvokerProvider actionInvokerProvider)
        {
            var proxyType = FindProxyTypeFromAssembly();
            if (proxyType == null)
            {
                var message = $"找不到{typeof(THttpApi)}的代理类：{GetErrorReason()}";
                throw new ProxyTypeCreateException(typeof(THttpApi), message);
            }

            var apiMethods = FindApiMethods(proxyType);

            this.actionInvokers = apiMethods
                .Select(item => apiActionDescriptorProvider.CreateActionDescriptor(item, typeof(THttpApi)))
                .Select(item => actionInvokerProvider.CreateActionInvoker(item))
                .ToArray();

            this.activator = (interceptor, invokers) => (THttpApi)Activator.CreateInstance(proxyType, interceptor, invokers);
        }

        /// <summary>
        /// 创建接口的实例
        /// </summary>
        /// <param name="apiInterceptor">接口拦截器</param>
        /// <returns></returns>
        public THttpApi CreateInstance(IHttpApiInterceptor apiInterceptor)
        {
            return this.activator.Invoke(apiInterceptor, this.actionInvokers);
        }

        /// <summary>
        /// 查找接口的Api方法 
        /// </summary>
        /// <returns></returns>
        private static MethodInfo[] FindApiMethods(Type proxyType)
        {
            var apiMethods = HttpApi.FindApiMethods(typeof(THttpApi));
            var proxyMethods = proxyType.GetMethods();

            var methods = from a in apiMethods
                          join p in proxyMethods
                          on new MethodFeature(a) equals new MethodFeature(p)
                          let methodAttr = p.GetCustomAttribute<HttpApiProxyMethodAttribute>()
                          where methodAttr != null
                          orderby methodAttr.Index
                          select a;

            return methods.ToArray();
        }

        /// <summary>
        /// 从接口所在程序集查找代理类
        /// </summary> 
        /// <returns></returns>
        private static Type? FindProxyTypeFromAssembly()
        {
            var interfaceType = typeof(THttpApi);
            var httpApiType = interfaceType.IsGenericType
                ? interfaceType.GetGenericTypeDefinition()
                : interfaceType;

            foreach (var proxyType in interfaceType.Assembly.GetTypes())
            {
                if (proxyType.IsClass == false)
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

        /// <summary>
        /// 获取错误原因
        /// </summary>
        /// <returns></returns>
        private static string GetErrorReason()
        {
            var assembly = typeof(THttpApi).Assembly;
            const string SourceGenerator = "WebApiClientCore.Extensions.SourceGenerator";
            if (assembly.GetReferencedAssemblies().Any(a => a.Name == SourceGenerator))
            {
                return "需要更新你的Visual Studio或msbuild工具到新版本";
            }
            else
            {
                return $"项目{assembly.GetName().Name}需要引用{SourceGenerator}";
            }
        }
    }
}
