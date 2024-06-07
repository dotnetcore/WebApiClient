using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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
        private static readonly Type? _proxyClassType = SourceGeneratorProxyClassFinder.Find(typeof(THttpApi));

        /// <summary>
        /// 获取是否支持
        /// </summary>
        public static bool IsSupported => _proxyClassType != null;

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
            var httpApiType = typeof(THttpApi);
            var proxyClassType = _proxyClassType;
            if (proxyClassType == null)
            {
                var message = $"找不到{httpApiType}的代理类";
                throw new ProxyTypeCreateException(httpApiType, message);
            }

            this.actionInvokers = FindApiMethods(httpApiType, proxyClassType)
                .Select(item => apiActionDescriptorProvider.CreateActionDescriptor(item, httpApiType))
                .Select(actionInvokerProvider.CreateActionInvoker)
                .ToArray();

            this.activator = RuntimeFeature.IsDynamicCodeSupported
                ? LambdaUtil.CreateCtorFunc<IHttpApiInterceptor, ApiActionInvoker[], THttpApi>(proxyClassType)
                : (interceptor, invokers) => proxyClassType.CreateInstance<THttpApi>(interceptor, invokers);
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
        /// 查找接口的方法
        /// </summary>
        /// <param name="httpApiType">接口类型</param> 
        /// <param name="proxyClassType">接口的实现类型</param>
        /// <returns></returns>
        private static IEnumerable<MethodInfo> FindApiMethods(Type httpApiType, Type proxyClassType)
        {
            var apiMethods = HttpApi.FindApiMethods(httpApiType)
                .Select(item => new MethodFeature(item, isProxyMethod: false))
                .ToArray();

            var classMethods = proxyClassType.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Select(item => new MethodFeature(item, isProxyMethod: true))
                .Where(item => item.Index >= 0)
                .ToArray();

            if (apiMethods.Length != classMethods.Length)
            {
                var message = $"接口类型{httpApiType}的代理类{proxyClassType}和当前版本不兼容，请将{httpApiType.Assembly.GetName().Name}项目所依赖的WebApiClientCore更新到版本v{typeof(SourceGeneratorHttpApiActivator<>).Assembly.GetName().Version}";
                throw new ProxyTypeException(httpApiType, message);
            }

            // 按照Index特征对apiMethods进行排序
            return from a in apiMethods
                   join c in classMethods
                   on a equals c
                   orderby c.Index
                   select a.Method;
        }

        /// <summary>
        /// 表示MethodInfo的特征
        /// </summary>
        [DebuggerDisplay("[{Index,nq}] {declaringType.FullName,nq}.{name,nq}")]
        private sealed class MethodFeature : IEquatable<MethodFeature>
        {
            private readonly string name;
            private readonly Type? declaringType;

            public MethodInfo Method { get; }

            public int Index { get; }

            /// <summary>
            /// MethodInfo的特征
            /// </summary>
            /// <param name="method"></param>
            /// <param name="isProxyMethod"></param>
            public MethodFeature(MethodInfo method, bool isProxyMethod)
            {
                this.Method = method;

                var attribute = default(HttpApiProxyMethodAttribute);
                if (isProxyMethod)
                {
                    attribute = method.GetCustomAttribute<HttpApiProxyMethodAttribute>();
                }

                if (attribute == null)
                {
                    this.Index = -1;
                    this.declaringType = method.DeclaringType;
                    this.name = method.Name;
                }
                else
                {
                    this.Index = attribute.Index;
                    this.declaringType = attribute.DeclaringType;
                    this.name = attribute.Name;
                }
            }

            /// <summary>
            /// 比较方法原型是否相等
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public bool Equals(MethodFeature? other)
            {
                if (other == null ||
                    this.name != other.name ||
                    this.declaringType != other.declaringType)
                {
                    return false;
                }

                var x = this.Method;
                var y = other.Method;
                if (x.ReturnType != y.ReturnType)
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

                hashCode.Add(this.declaringType);
                hashCode.Add(this.name);
                hashCode.Add(this.Method.ReturnType);
                foreach (var parameter in this.Method.GetParameters())
                {
                    hashCode.Add(parameter.ParameterType);
                }
                return hashCode.ToHashCode();
            }

        }
    }
}
