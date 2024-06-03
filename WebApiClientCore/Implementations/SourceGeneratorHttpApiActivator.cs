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
    public class SourceGeneratorHttpApiActivator<
#if NET5_0_OR_GREATER
        [System.Diagnostics.CodeAnalysis.DynamicallyAccessedMembers(System.Diagnostics.CodeAnalysis.DynamicallyAccessedMemberTypes.PublicConstructors)]
#endif
    THttpApi> : IHttpApiActivator<THttpApi>
    {
        private readonly ApiActionInvoker[] actionInvokers;
        private readonly Func<IHttpApiInterceptor, ApiActionInvoker[], THttpApi> activator;
        private static readonly Type? proxyClassType = FindProxyTypeFromAssembly();

        /// <summary>
        /// 获取是否支持
        /// </summary>
        public static bool IsSupported => proxyClassType != null;

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
            var proxyType = proxyClassType;
            if (proxyType == null)
            {
                var message = $"找不到{typeof(THttpApi)}的代理类";
                throw new ProxyTypeCreateException(typeof(THttpApi), message);
            }

            this.actionInvokers = FindApiMethods(proxyType)
                .Select(item => apiActionDescriptorProvider.CreateActionDescriptor(item, typeof(THttpApi)))
                .Select(actionInvokerProvider.CreateActionInvoker)
                .ToArray();

            this.activator = LambdaUtil.CreateCtorFunc<IHttpApiInterceptor, ApiActionInvoker[], THttpApi>(proxyType);
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
            foreach (var proxyType in interfaceType.Assembly.GetTypes())
            {
                if (proxyType.IsClass == false)
                {
                    continue;
                }

                var proxyClassAttr = proxyType.GetCustomAttribute<HttpApiProxyClassAttribute>();
                if (proxyClassAttr == null || proxyClassAttr.HttpApiType != interfaceType)
                {
                    continue;
                }

                return proxyType;
            }

            return null;
        }

        /// <summary>
        /// 表示MethodInfo的特征
        /// </summary>
        private sealed class MethodFeature : IEquatable<MethodFeature>
        {
            private readonly MethodInfo method;

            /// <summary>
            /// MethodInfo的特征
            /// </summary>
            /// <param name="method"></param>
            public MethodFeature(MethodInfo method)
            {
                this.method = method;
            }

            /// <summary>
            /// 比较方法原型是否相等
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public bool Equals(MethodFeature? other)
            {
                if (other == null)
                {
                    return false;
                }

                var x = this.method;
                var y = other.method;

                if (x.Name != y.Name || x.ReturnType != y.ReturnType)
                {
                    return false;
                }

                var xParameterTypes = x.GetParameters().Select(p => p.ParameterType);
                var yParameterTypes = y.GetParameters().Select(p => p.ParameterType);
                return xParameterTypes.SequenceEqual(yParameterTypes);
            }


            public override bool Equals(object? obj)
            {
                return this.Equals(obj as MethodFeature);
            }

            /// <summary>
            /// 获取哈希
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                var hashCode = new HashCode();
                hashCode.Add(this.method.Name);
                hashCode.Add(this.method.ReturnType);
                foreach (var parameter in this.method.GetParameters())
                {
                    hashCode.Add(parameter.ParameterType);
                }
                return hashCode.ToHashCode();
            }

        }
    }
}
