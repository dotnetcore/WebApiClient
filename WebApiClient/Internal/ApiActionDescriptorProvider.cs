using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebApiClient.Attributes;
using WebApiClient.Contexts;
using WebApiClient.DataAnnotations;

namespace WebApiClient
{
    /// <summary>
    /// 提供Api描述的创建与获取
    /// </summary>
    static class ApiActionDescriptorProvider
    {
        /// <summary>
        /// 缓存字典
        /// </summary>
        private static readonly ConcurrentCache<MethodInfo, ApiActionDescriptor> cache;

        /// <summary>
        /// Api描述的创建与获取
        /// </summary>
        static ApiActionDescriptorProvider()
        {
            cache = new ConcurrentCache<MethodInfo, ApiActionDescriptor>();
        }

        /// <summary>
        /// 返回方法的ApiActionDescriptor
        /// </summary>
        /// <param name="method">接口的方法</param>
        /// <returns></returns>
        public static ApiActionDescriptor GetDescriptor(MethodInfo method)
        {
            return GetDescriptor(method, null);
        }

        /// <summary>
        /// 返回方法的ApiActionDescriptor
        /// </summary>
        /// <param name="method">接口的方法</param>
        /// <param name="parameterValues">参数值集合</param>
        /// <returns></returns>
        public static ApiActionDescriptor GetDescriptor(MethodInfo method, object[] parameterValues)
        {
            return cache.GetOrAdd(method, GetDescriptorNoCache).Clone(parameterValues);
        }

        /// <summary>
        /// 从拦截内容获得ApiActionDescriptor
        /// </summary>
        /// <param name="method">接口的方法</param>
        /// <returns></returns>
        private static ApiActionDescriptor GetDescriptorNoCache(MethodInfo method)
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
                Member = method,
                Name = method.Name,
                Filters = filterAttributes,
                Attributes = actionAttributes,
                Return = method.GetReturnDescriptor(),
                Parameters = method.GetParameters().Select(p => p.GetParameterDescriptor()).ToArray()
            };
        }

        /// <summary>
        /// 生成ApiParameterDescriptor
        /// </summary>
        /// <param name="parameter">参数信息</param>
        /// <returns></returns>
        private static ApiParameterDescriptor GetParameterDescriptor(this ParameterInfo parameter)
        {
            var parameterType = parameter.ParameterType;
            var parameterAlias = parameter.GetCustomAttribute(typeof(AliasAsAttribute)) as AliasAsAttribute;
            var parameterName = parameterAlias == null ? parameter.Name : parameterAlias.Name;

            var defined = parameter.GetAttributes<IApiParameterAttribute>(true);
            var attributes = HttpApiConfig.DefaultApiParameterAttributeProvider.GetAttributes(parameterType, defined);
            var validationAttributes = parameter.GetCustomAttributes<ValidationAttribute>(true).ToArray();

            return new ApiParameterDescriptor
            {
                Value = null,
                Member = parameter,
                Name = parameterName,
                Index = parameter.Position,
                Attributes = attributes,
                ParameterType = parameterType,
                ValidationAttributes = validationAttributes
            };
        }

        /// <summary>
        /// 生成ApiReturnDescriptor
        /// </summary>
        /// <param name="method">方法信息</param>
        /// <returns></returns>
        private static ApiReturnDescriptor GetReturnDescriptor(this MethodInfo method)
        {
            var returnAttribute = method.FindDeclaringAttribute<IApiReturnAttribute>(true);
            if (returnAttribute == null)
            {
                returnAttribute = new AutoReturnAttribute();
            }

            var dataType = method.ReturnType.GetGenericArguments().FirstOrDefault();
            var dataTypeDefinition = method.ReturnType.GetGenericTypeDefinition();

            var descriptor = new ApiReturnDescriptor
            {
                Attribute = returnAttribute,
                ReturnType = method.ReturnType,
                DataType = new DataTypeDescriptor(dataType),
                IsTaskDefinition = dataTypeDefinition == typeof(Task<>),
                IsITaskDefinition = dataTypeDefinition == typeof(ITask<>)
            };
            return descriptor;
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
