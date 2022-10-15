using System;
using System.Linq;
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
        /// 代理类型
        /// </summary>
        private static readonly Type? proxyType = FindProxyTypeFromAssembly();

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
        /// 查找接口的Api方法
        /// 用于创建代理对象的ApiActionInvoker
        /// </summary>
        /// <returns></returns>
        protected override MethodInfo[] FindApiMethods()
        {
            if (proxyType != null)
            {
                var apiMethods = base.FindApiMethods();
                var proxyMethods = proxyType.GetMethods();
                var methods = from a in apiMethods
                              join p in proxyMethods
                              on new MethodFeature(a) equals new MethodFeature(p)
                              let attr = p.GetCustomAttribute<HttpApiProxyMethodAttribute>()
                              let index = attr == null ? 0 : attr.Index
                              orderby index
                              select a;

                return methods.ToArray();
            }

            return base.FindApiMethods();
        }

        /// <summary>
        /// 创建实例工厂
        /// </summary>
        /// <exception cref="ProxyTypeCreateException"></exception>
        /// <returns></returns>
        protected override Func<IHttpApiInterceptor, ApiActionInvoker[], THttpApi> CreateFactory()
        {
            if (proxyType != null)
            {
                return LambdaUtil.CreateCtorFunc<IHttpApiInterceptor, ApiActionInvoker[], THttpApi>(proxyType);
            }

            var message = $"找不到{typeof(THttpApi)}的代理类：{GetErrorReason()}";
            throw new ProxyTypeCreateException(typeof(THttpApi), message);
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
