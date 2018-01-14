using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
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
            var actionAttributes = method
                .FindDeclaringAttributes<IApiActionAttribute>(true)
                .Distinct(new MultiplableComparer<IApiActionAttribute>())
                .OrderBy(item => item.OrderIndex)
                .ToArray();

            var filterAttributes = method
                .FindDeclaringAttributes<IApiActionFilterAttribute>(true)
                .Distinct(new MultiplableComparer<IApiActionFilterAttribute>())
                .OrderBy(item => item.OrderIndex)
                .ToArray();

            return new ApiActionDescriptor
            {
                Name = method.Name,
                Filters = filterAttributes,
                Return = GetReturnDescriptor(method),
                Attributes = actionAttributes,
                Parameters = method.GetParameters().Select(p => GetParameterDescriptor(p)).ToArray()
            };
        }

        /// <summary>
        /// 生成ApiParameterDescriptor
        /// </summary>
        /// <param name="parameter">参数信息</param>
        /// <returns></returns>
        private static ApiParameterDescriptor GetParameterDescriptor(ParameterInfo parameter)
        {
            var parameterType = parameter.ParameterType;
            var parameterAlias = parameter.GetCustomAttribute(typeof(AliasAsAttribute)) as AliasAsAttribute;
            var parameterName = parameterAlias == null ? parameter.Name : parameterAlias.Name;
            var isHttpContent = parameterType.IsInheritFrom<HttpContent>();
            var isApiParameterable = parameterType.IsInheritFrom<IApiParameterable>() || parameterType.IsInheritFrom<IEnumerable<IApiParameterable>>();

            var defined = parameter.GetAttributes<IApiParameterAttribute>(true);
            var attributes = new ParameterAttributeCollection(defined);

            if (isApiParameterable == true)
            {
                attributes.Add(new ParameterableAttribute());
            }
            else if (isHttpContent == true)
            {
                attributes.AddIfNotExists(new HttpContentAttribute());
            }
            else if (attributes.Count == 0)
            {
                attributes.Add(new PathQueryAttribute());
            }

            return new ApiParameterDescriptor
            {
                Attributes = attributes.ToArray(),
                Index = parameter.Position,
                Name = parameterName,
                ParameterType = parameterType,
                Value = null,
            };
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

            var dataType = method.ReturnType.GetGenericArguments().FirstOrDefault();
            var descriptor = new ApiReturnDescriptor
            {
                Attribute = returnAttribute,
                ReturnType = method.ReturnType,
                GenericType = method.ReturnType.GetGenericTypeDefinition(),
                DataType = dataType,
                ITaskCtor = ApiTask.GetConstructor(dataType),
            };
            return descriptor;
        }

        /// <summary>
        /// 表示参数特性集合
        /// </summary>
        private class ParameterAttributeCollection
        {
            /// <summary>
            /// 特性列表
            /// </summary>
            private readonly List<IApiParameterAttribute> attribueList = new List<IApiParameterAttribute>();

            /// <summary>
            /// 获取元素数量
            /// </summary>
            public int Count
            {
                get
                {
                    return this.attribueList.Count;
                }
            }

            /// <summary>
            /// 参数特性集合
            /// </summary>
            /// <param name="defined">声明的特性</param>
            public ParameterAttributeCollection(IEnumerable<IApiParameterAttribute> defined)
            {
                this.attribueList.AddRange(defined);
            }

            /// <summary>
            /// 添加新特性
            /// </summary>
            /// <param name="attribute"></param>
            public void Add(IApiParameterAttribute attribute)
            {
                this.attribueList.Add(attribute);
            }

            /// <summary>
            /// 添加新特性
            /// </summary>
            /// <param name="attribute"></param>
            /// <returns></returns>
            public bool AddIfNotExists(IApiParameterAttribute attribute)
            {
                var type = attribute.GetType();
                if (this.attribueList.Any(item => item.GetType() == type) == true)
                {
                    return false;
                }

                this.attribueList.Add(attribute);
                return true;
            }

            /// <summary>
            /// 转换为数组
            /// </summary>
            /// <returns></returns>
            public IApiParameterAttribute[] ToArray()
            {
                return this.attribueList.ToArray();
            }
        }

        /// <summary>
        /// 是否允许重复的特性比较器
        /// </summary>
        private class MultiplableComparer<TAttributeMultiplable> : IEqualityComparer<TAttributeMultiplable> where TAttributeMultiplable : IAttributeMultiplable
        {
            /// <summary>
            /// 是否相等
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <returns></returns>
            public bool Equals(TAttributeMultiplable x, TAttributeMultiplable y)
            {
                // 如果其中一个不允许重复，返回true将y过滤
                return x.AllowMultiple == false || y.AllowMultiple == false;
            }

            /// <summary>
            /// 获取哈希码
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns> 
            public int GetHashCode(TAttributeMultiplable obj)
            {
                return obj.GetType().GetHashCode();
            }
        }
    }
}
