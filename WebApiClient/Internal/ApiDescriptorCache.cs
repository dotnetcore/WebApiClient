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

namespace WebApiClient
{
    /// <summary>
    /// 提供Api描述的缓存
    /// </summary>
    static class ApiDescriptorCache
    {
        /// <summary>
        /// 缓存字典
        /// </summary>
        private static readonly ConcurrentDictionary<MethodInfo, ApiActionDescriptor> cache;

        /// <summary>
        /// Castle相关上下文
        /// </summary>
        static ApiDescriptorCache()
        {
            cache = new ConcurrentDictionary<MethodInfo, ApiActionDescriptor>();
        }

        /// <summary>
        /// 从缓存获得ApiActionDescriptor
        /// </summary>
        /// <param name="method">接口的方法</param>
        /// <returns></returns>
        public static ApiActionDescriptor GetApiActionDescriptor(MethodInfo method)
        {
            return cache.GetOrAdd(method, GetActionDescriptor);
        }

        /// <summary>
        /// 从拦截内容获得ApiActionDescriptor
        /// </summary>
        /// <param name="method">接口的方法</param>
        /// <returns></returns>
        private static ApiActionDescriptor GetActionDescriptor(MethodInfo method)
        {
            if (method.ReturnType.IsGenericType == false || method.ReturnType.GetGenericTypeDefinition() != typeof(Task<>))
            {
                var message = string.Format("接口{0}返回类型应该是Task<{1}>", method.Name, method.ReturnType.Name);
                throw new NotSupportedException(message);
            }

            var actionAttributes = method
                .FindDeclaringAttributes<IApiActionAttribute>(true)
                .Distinct(new AttributeComparer<IApiActionAttribute>())
                .OrderBy(item => item.OrderIndex)
                .ToArray();

            var filterAttributes = method
                .FindDeclaringAttributes<IApiActionFilterAttribute>(true)
                .Distinct(new AttributeComparer<IApiActionFilterAttribute>())
                .OrderBy(item => item.OrderIndex)
                .ToArray();

            return new ApiActionDescriptor
            {
                Name = method.Name,
                Filters = filterAttributes,
                Return = GetReturnDescriptor(method),
                Attributes = actionAttributes,
                Parameters = method.GetParameters().Select((p, i) => GetParameterDescriptor(p, i)).ToArray()
            };
        }

        /// <summary>
        /// 生成ApiParameterDescriptor
        /// </summary>
        /// <param name="parameter">参数信息</param>
        /// <param name="index">参数索引</param>
        /// <returns></returns>
        private static ApiParameterDescriptor GetParameterDescriptor(ParameterInfo parameter, int index)
        {
            var parameterType = parameter.ParameterType;
            if (parameterType.IsByRef == true)
            {
                var message = string.Format("接口参数不支持ref/out修饰：{0}", parameter);
                throw new NotSupportedException(message);
            }

            var descriptor = new ApiParameterDescriptor
            {
                Value = null,
                Name = parameter.Name,
                Index = index,
                ParameterType = parameterType,
                IsApiParameterable = parameterType.IsInheritFrom<IApiParameterable>() || parameterType.IsInheritFrom<IEnumerable<IApiParameterable>>(),
                IsHttpContent = parameterType.IsInheritFrom<HttpContent>(),
                IsSimpleType = parameterType.IsSimple(),
                IsEnumerable = parameterType.IsInheritFrom<IEnumerable>(),
                IsDictionaryOfObject = parameterType.IsInheritFrom<IDictionary<string, object>>(),
                IsDictionaryOfString = parameterType.IsInheritFrom<IDictionary<string, string>>(),
                Attributes = parameter.GetAttributes<IApiParameterAttribute>(true).ToArray()
            };

            if (descriptor.Attributes.Length == 0)
            {
                if (descriptor.IsApiParameterable == true)
                {
                    descriptor.Attributes = new[] { new ParameterableAttribute() };
                }
                else if (descriptor.IsHttpContent == true)
                {
                    descriptor.Attributes = new[] { new HttpContentAttribute() };
                }
                else
                {
                    descriptor.Attributes = new[] { new PathQueryAttribute() };
                }
            }
            return descriptor;
        }

        /// <summary>
        /// 生成ApiReturnDescriptor
        /// </summary>
        /// <param name="method">方法信息</param>
        /// <returns></returns>
        private static ApiReturnDescriptor GetReturnDescriptor(MethodInfo method)
        {
            var returnAttribute = method.FindDeclaringAttribute<IApiReturnAttribute>(true);
            if (returnAttribute == null)
            {
                returnAttribute = new AutoReturnAttribute();
            }

            return new ApiReturnDescriptor
            {
                Attribute = returnAttribute,
                TaskType = method.ReturnType,
                DataType = method.ReturnType.GetGenericArguments().FirstOrDefault(),
            };
        }

        /// <summary>
        /// 特性比较器
        /// </summary>
        private class AttributeComparer<T> : IEqualityComparer<T> where T : IAttributeMultiplable
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
    }
}
