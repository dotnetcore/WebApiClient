using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示Action描述
    /// </summary>
    [DebuggerDisplay("Name = {Name}")]
    public class ApiActionDescriptor
    {
        /// <summary>
        /// 获取方法的唯一标识
        /// </summary>
        public string Id { get; protected set; }

        /// <summary>
        /// 获取所在接口类型
        /// 这个值不一定是声明方法的接口类型
        /// </summary>
        public Type InterfaceType { get; protected set; }

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
        public IApiCacheAttribute? CacheAttribute { get; protected set; }

        /// <summary>
        /// 获取Api关联的特性
        /// </summary>
        public IReadOnlyList<IApiActionAttribute> Attributes { get; protected set; }

        /// <summary>
        /// 获取Api关联的过滤器特性
        /// </summary>
        public IReadOnlyList<IApiFilterAttribute> FilterAttributes { get; protected set; }


        /// <summary>
        /// 获取Api的参数描述
        /// </summary>
        public IReadOnlyList<ApiParameterDescriptor> Parameters { get; protected set; }

        /// <summary>
        /// 获取Api的返回描述
        /// </summary>
        public ApiReturnDescriptor Return { get; protected set; }

        /// <summary>
        /// 获取自定义数据存储的字典
        /// </summary>
        public ConcurrentDictionary<object, object> Properties { get; protected set; }

        /// <summary>
        /// 请求Api描述
        /// </summary>
        /// <param name="method">接口的方法</param>
        /// <param name="interfaceType">接口类型</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public ApiActionDescriptor(MethodInfo method, Type? interfaceType = default)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            var methodAttributes = method.GetAttributes<IApiActionAttribute>(false);

            var interfaceAttributes = interfaceType == null
                ? Enumerable.Empty<IApiActionAttribute>()
                : interfaceType.GetAttributes<IApiActionAttribute>(false);

            var decalringAttributes = method.DeclaringType == null
                ? Enumerable.Empty<IApiActionAttribute>()
                : method.DeclaringType.GetAttributes<IApiActionAttribute>(false);

            // 接口特性优先于方法所在类型的特性
            var actionAttributes = methodAttributes
                .Concat(interfaceAttributes)
                .Concat(decalringAttributes)
                .Distinct(new MultiplableComparer<IApiActionAttribute>())
                .OrderBy(item => item.OrderIndex)
                .ToReadOnlyList();

            var filterAttributes = method
                .GetAttributes<IApiFilterAttribute>(true)
                .Distinct(new MultiplableComparer<IApiFilterAttribute>())
                .OrderBy(item => item.OrderIndex)
                .Where(item => item.Enable)
                .ToReadOnlyList();

            this.Id = Guid.NewGuid().ToString();
            this.InterfaceType = interfaceType ?? (method.DeclaringType ?? throw new ArgumentException(nameof(method)));

            this.Member = method;
            this.Name = method.Name;
            this.Attributes = actionAttributes;
            this.CacheAttribute = method.GetAttribute<IApiCacheAttribute>();
            this.FilterAttributes = filterAttributes;

            this.Return = new ApiReturnDescriptor(method);
            this.Parameters = method.GetParameters().Select(p => new ApiParameterDescriptor(p)).ToReadOnlyList();
            this.Properties = new ConcurrentDictionary<object, object>();
        }
    }
}
