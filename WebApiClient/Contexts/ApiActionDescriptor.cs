using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace WebApiClient.Contexts
{
    /// <summary>
    /// 表示请求Api描述
    /// </summary>
    [DebuggerDisplay("Name = {Name}")]
    public class ApiActionDescriptor
    {
        /// <summary>
        /// 获取Api名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 获取关联的方法信息
        /// </summary>
        public MethodInfo Member { get; private set; }

        /// <summary>
        /// 获取Api关联的特性
        /// </summary>
        public IApiActionAttribute[] Attributes { get; private set; }

        /// <summary>
        /// 获取Api关联的过滤器特性
        /// </summary>
        public IApiActionFilterAttribute[] Filters { get; private set; }

        /// <summary>
        /// 获取Api的参数描述
        /// </summary>
        public ApiParameterDescriptor[] Parameters { get; private set; }

        /// <summary>
        /// 获取Api的返回描述
        /// </summary>
        public ApiReturnDescriptor Return { get; private set; }

        /// <summary>
        /// 克隆并设置新的参数值
        /// </summary>
        /// <param name="parameterValues">新的参数值集合</param>
        /// <returns></returns>
        public ApiActionDescriptor Clone(object[] parameterValues)
        {
            return new ApiActionDescriptor
            {
                Name = this.Name,
                Member = this.Member,
                Return = this.Return,
                Filters = this.Filters,
                Attributes = this.Attributes,
                Parameters = this.Parameters.Select((p, i) => p.Clone(parameterValues[i])).ToArray()
            };
        }

        /// <summary>
        /// 创建ApiActionDescriptor
        /// </summary>
        /// <param name="method">接口的方法</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <returns></returns>
        public static ApiActionDescriptor Create(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

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
                Return = ApiReturnDescriptor.Create(method),
                Parameters = method.GetParameters().Select(p => ApiParameterDescriptor.Create(p)).ToArray()
            };
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
