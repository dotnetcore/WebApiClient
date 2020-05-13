using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace WebApiClientCore
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
        public string Name { get; protected set; }

        /// <summary>
        /// 获取关联的方法信息
        /// </summary>
        public MethodInfo Member { get; protected set; }

        /// <summary>
        /// 获取Api关联的缓存特性
        /// </summary>
        public IApiActionCacheAttribute Cache { get; protected set; }

        /// <summary>
        /// 获取Api关联的特性
        /// </summary>
        public IReadOnlyList<IApiActionAttribute> Attributes { get; protected set; }

        /// <summary>
        /// 获取Api关联的过滤器特性
        /// </summary>
        public IReadOnlyList<IApiActionFilterAttribute> Filters { get; protected set; }

        /// <summary>
        /// 获取Api的参数描述
        /// </summary>
        public IReadOnlyList<ApiParameterDescriptor> Parameters { get; protected set; }

        /// <summary>
        /// 获取Api的返回描述
        /// </summary>
        public ApiReturnDescriptor Return { get; protected set; }

        /// <summary>
        /// 请求Api描述
        /// </summary>
        /// <param name="method">接口的方法</param>
        /// <exception cref="ArgumentNullException"></exception>
        public ApiActionDescriptor(MethodInfo method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            var actionAttributes = method
                .FindDeclaringAttributes<IApiActionAttribute>(true)
                .Distinct(new MultiplableComparer<IApiActionAttribute>())
                .OrderBy(item => item.OrderIndex)
                .ToReadOnlyList();

            var filterAttributes = method
                .FindDeclaringAttributes<IApiActionFilterAttribute>(true)
                .Distinct(new MultiplableComparer<IApiActionFilterAttribute>())
                .OrderBy(item => item.OrderIndex)
                .ToReadOnlyList();


            this.Member = method;
            this.Name = method.Name;
            this.Cache = method.GetAttribute<IApiActionCacheAttribute>(true);
            this.Filters = filterAttributes;
            this.Attributes = actionAttributes;
            this.Return = new ApiReturnDescriptor(method);
            this.Parameters = method.GetParameters().Select(p => new ApiParameterDescriptor(p)).ToReadOnlyList();
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
