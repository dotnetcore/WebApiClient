using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示httpApi方法调用的拦截器
    /// </summary>
    class ActionInterceptor : IActionInterceptor
    {
        /// <summary>
        /// action执行器的缓存
        /// </summary>
        private static readonly ConcurrentCache<Method, IActionInvoker> invokerCache = new ConcurrentCache<Method, IActionInvoker>();

        /// <summary>
        /// 服务上下文
        /// </summary>
        private readonly ServiceContext context;

        /// <summary>
        /// httpApi方法调用的拦截器
        /// </summary>
        /// <param name="context">服务上下文</param> 
        public ActionInterceptor(ServiceContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// 拦截方法的调用
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <param name="method">接口的方法</param>
        /// <param name="arguments">方法的参数集合</param>
        /// <returns></returns>
        public object Intercept(Type interfaceType, MethodInfo method, object[] arguments)
        {
            var key = new Method(interfaceType, method);
            var invoker = invokerCache.GetOrAdd(key, CreateActionInvoker);
            return invoker.Invoke(this.context, arguments);
        }

        /// <summary>
        /// 创建action执行器
        /// </summary>
        /// <param name="method">接口的方法</param>
        /// <returns></returns>
        private static IActionInvoker CreateActionInvoker(Method method)
        {
            var apiAction = new ApiActionDescriptor(method.InterfaceType, method.MethodInfo);
            return new MultiplexedActionInvoker(apiAction);
        }

        /// <summary>
        /// 表示方法信息
        /// </summary>
        private struct Method : IEquatable<Method>
        {
            /// <summary>
            /// 所在接口
            /// </summary>
            public Type InterfaceType { get; }

            /// <summary>
            /// 方法信息
            /// </summary>
            public MethodInfo MethodInfo { get; }

            /// <summary>
            /// 方法信息
            /// </summary>
            /// <param name="interfaceType"></param>
            /// <param name="methodInfo"></param>
            public Method(Type interfaceType, MethodInfo methodInfo)
            {
                this.InterfaceType = interfaceType;
                this.MethodInfo = methodInfo;
            }

            /// <summary>
            /// 是否相等
            /// </summary>
            /// <param name="other"></param>
            /// <returns></returns>
            public bool Equals([AllowNull] Method other)
            {
                return ReferenceEquals(this.MethodInfo, other.MethodInfo);
            }

            /// <summary>
            /// 获取哈希
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                return HashCode.Combine(this.MethodInfo);
            }
        }
    }
}
