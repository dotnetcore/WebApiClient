using Castle.DynamicProxy;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using WebApiClient.Attributes;
using WebApiClient.Contexts;

namespace WebApiClient
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
        public IApiReturnAttribute ApiReturnAttribute { get; private set; }

        /// <summary>
        /// 获取ApiActionFilterAttribute
        /// </summary>
        public IApiActionFilterAttribute[] ApiActionFilterAttributes { get; set; }

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
            var hostAttribute = invocation.Proxy.GetType().GetCustomAttribute<HttpHostAttribute>();
            if (hostAttribute == null)
            {
                hostAttribute = method.FindDeclaringAttribute<HttpHostAttribute>(false);
            }
            if (hostAttribute == null)
            {
                throw new HttpRequestException("未指定HttpHostAttribute");
            }

            var returnAttribute = method.FindDeclaringAttribute<IApiReturnAttribute>(true);
            if (returnAttribute == null)
            {
                returnAttribute = new AutoReturnAttribute();
            }

            var filterAttributes = method
                .FindDeclaringAttributes<IApiActionFilterAttribute>(true)
                .Distinct(new AttributeComparer<IApiActionFilterAttribute>())
                .OrderBy(item => item.OrderIndex)
                .ToArray();

            return new CastleContext
            {
                HostAttribute = hostAttribute,
                ApiReturnAttribute = returnAttribute,
                ApiActionFilterAttributes = filterAttributes,
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
            if (method.ReturnType.IsGenericType == false || method.ReturnType.GetGenericTypeDefinition() != typeof(Task<>))
            {
                var message = string.Format("接口{0}返回类型应该是Task<{1}>", method.Name, method.ReturnType.Name);
                throw new NotSupportedException(message);
            }

            var descriptor = new ApiActionDescriptor
            {
                Name = method.Name,
                ReturnTaskType = method.ReturnType,
                ReturnDataType = method.ReturnType.GetGenericArguments().FirstOrDefault(),
                Attributes = method.FindDeclaringAttributes<IApiActionAttribute>(true).Distinct(new AttributeComparer<IApiActionAttribute>()).ToArray(),
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
            if (parameter.ParameterType.IsByRef == true)
            {
                var message = string.Format("接口参数不支持ref/out修饰：{0}", parameter);
                throw new NotSupportedException(message);
            }

            var parameterDescriptor = new ApiParameterDescriptor
            {
                Name = parameter.Name,
                Index = index,
                ParameterType = parameter.ParameterType,
                IsSimpleType = parameter.ParameterType.IsSimple(),
                IsEnumerable = parameter.ParameterType.IsInheritFrom<IEnumerable>(),
                Attributes = parameter.GetAttributes<IApiParameterAttribute>(true).ToArray()
            };

            if (parameterDescriptor.Attributes.Length == 0)
            {
                if (parameter.ParameterType.IsInheritFrom<IApiParameterable>() || parameter.ParameterType.IsEnumerable<IApiParameterable>())
                {
                    parameterDescriptor.Attributes = new[] { new ParameterableAttribute() };
                }
                else if (parameter.ParameterType.IsInheritFrom<HttpContent>())
                {
                    parameterDescriptor.Attributes = new[] { new HttpContentAttribute() };
                }
                else if (parameterDescriptor.Attributes.Length == 0)
                {
                    parameterDescriptor.Attributes = new[] { new PathQueryAttribute() };
                }
            }
            return parameterDescriptor;
        }

        /// <summary>
        /// 特性比较器
        /// </summary>
        private class AttributeComparer<T> : IEqualityComparer<T> where T : IAttributeAllowMultiple
        {
            /// <summary>
            /// 是否相等
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public bool Equals(T x, T y)
            {
                // 如果其中一个不允许重复，返回true将y过滤
                return x.AllowMultiple == false || y.AllowMultiple == false;
            }

            /// <summary>
            /// 获取哈希码
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns> 
            public int GetHashCode(T obj)
            {
                return obj.GetType().GetHashCode();
            }
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
