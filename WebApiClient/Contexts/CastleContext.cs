using Castle.DynamicProxy;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    /// <summary>
    /// 表示Castle相关上下文
    /// </summary>
    class CastleContext
    {
        /// <summary>
        /// 获取HttpHostAttribute
        /// </summary>
        public HttpHostAttribute HostAttribute { get; private set; }

        /// <summary>
        /// 获取ApiReturnAttribute
        /// </summary>
        public ApiReturnAttribute ApiReturnAttribute { get; private set; }

        /// <summary>
        /// 获取ApiActionDescriptor
        /// </summary>
        public ApiActionDescriptor ApiActionDescriptor { get; private set; }


        /// <summary>
        /// 缓存字典
        /// </summary>
        private static readonly ConcurrentDictionary<IInvocation, CastleContext> cache;

        /// <summary>
        /// Castle相关上下文
        /// </summary>
        static CastleContext()
        {
            CastleContext.cache = new ConcurrentDictionary<IInvocation, CastleContext>(new IInvocationComparer());
        }

        /// <summary>
        /// 从拦截内容获得
        /// 使用缓存
        /// </summary>
        /// <param name="invocation">拦截内容</param>
        /// <returns></returns>
        public static CastleContext From(IInvocation invocation)
        {
            return CastleContext.cache.GetOrAdd(invocation, CastleContext.GetContextNoCache);
        }

        /// <summary>
        /// 从拦截内容获得
        /// </summary>
        /// <param name="invocation">拦截内容</param>
        /// <returns></returns>
        private static CastleContext GetContextNoCache(IInvocation invocation)
        {
            var method = invocation.Method;
            var hostAttribute = CastleContext.GetAttributeFromMethodAndInterface<HttpHostAttribute>(method, false);
            if (hostAttribute == null)
            {
                hostAttribute = invocation.Proxy.GetType().GetCustomAttribute<HttpHostAttribute>();
            }
            if (hostAttribute == null)
            {
                throw new HttpRequestException("未指定HttpHostAttribute");
            }

            var returnAttribute = CastleContext.GetAttributeFromMethodAndInterface<ApiReturnAttribute>(method, true);
            if (returnAttribute == null)
            {
                returnAttribute = new DefaultReturnAttribute();
            }

            return new CastleContext
            {
                HostAttribute = hostAttribute,
                ApiReturnAttribute = returnAttribute,
                ApiActionDescriptor = CastleContext.GetActionDescriptor(invocation)
            };
        }

        /// <summary>
        /// 生成ApiActionDescriptor
        /// </summary>
        /// <param name="invocation">拦截内容</param>
        /// <returns></returns>
        private static ApiActionDescriptor GetActionDescriptor(IInvocation invocation)
        {
            var method = invocation.Method;
            var descriptor = new ApiActionDescriptor
            {
                Name = method.Name,
                ReturnTaskType = method.ReturnType,
                ReturnDataType = method.ReturnType.GetGenericArguments().FirstOrDefault(),
                Attributes = method.GetCustomAttributes<ApiActionAttribute>(true).ToArray(),
                Parameters = method.GetParameters().Select((p, i) => GetParameterDescriptor(p, i)).ToArray()
            };
            return descriptor;
        }

        /// <summary>
        /// 生成ApiParameterDescriptor
        /// </summary>
        /// <param name="parameter">参数信息</param>
        /// <param name="index">参数索引</param>
        /// <returns></returns>
        private static ApiParameterDescriptor GetParameterDescriptor(ParameterInfo parameter, int index)
        {
            var parameterDescriptor = new ApiParameterDescriptor
            {
                Name = parameter.Name,
                Index = index,
                ParameterType = parameter.ParameterType,
                IsSimpleType = CastleContext.IsSimple(parameter.ParameterType),
                Attributes = parameter.GetCustomAttributes<ApiParameterAttribute>(true).ToArray()
            };

            if (parameterDescriptor.Attributes.Length == 0)
            {
                parameterDescriptor.Attributes = new[] { new PathQueryAttribute() };
            }
            return parameterDescriptor;
        }

        /// <summary>
        /// 从方法或接口获取特性
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="method">方法</param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        private static TAttribute GetAttributeFromMethodAndInterface<TAttribute>(MethodInfo method, bool inherit) where TAttribute : Attribute
        {
            var attribute = method.GetCustomAttribute<TAttribute>(inherit);
            if (attribute == null)
            {
                attribute = method.DeclaringType.GetCustomAttribute<TAttribute>(inherit);
            }
            return attribute;
        }

        /// <summary>
        /// 获取是否为简单类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        private static bool IsSimple(Type type)
        {
            if (type.IsGenericType == true)
            {
                type = type.GetGenericArguments().FirstOrDefault();
            }

            if (type.IsPrimitive || type.IsEnum)
            {
                return true;
            }

            return type == typeof(string)
                || type == typeof(decimal)
                || type == typeof(DateTime)
                || type == typeof(Guid);
        }

        /// <summary>
        /// IInvocation对象的比较器
        /// </summary>
        private class IInvocationComparer : IEqualityComparer<IInvocation>
        {
            /// <summary>
            /// 是否相等
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public bool Equals(IInvocation x, IInvocation y)
            {
                return x.Method.Equals(y.Method);
            }

            /// <summary>
            /// 获取哈希码
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public int GetHashCode(IInvocation obj)
            {
                return obj.Method.GetHashCode();
            }
        }
    }
}
