using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示Action描述
    /// </summary>
    public abstract class ApiActionDescriptor
    {
        /// <summary>
        /// 获取所在接口类型
        /// 这个值不一定是声明方法的接口类型
        /// </summary>
        public abstract Type InterfaceType { get; protected set; }

        /// <summary>
        /// 获取Api名称
        /// </summary>
        public abstract string Name { get; protected set; }

        /// <summary>
        /// 获取关联的方法信息
        /// </summary>
        public abstract MethodInfo Member { get; protected set; }

        /// <summary>
        /// 获取Api关联的缓存特性
        /// </summary>
        public abstract IApiCacheAttribute? CacheAttribute { get; protected set; }

        /// <summary>
        /// 获取Api关联的特性
        /// </summary>
        public abstract IReadOnlyList<IApiActionAttribute> Attributes { get; protected set; }

        /// <summary>
        /// 获取Api关联的过滤器特性
        /// </summary>
        public abstract IReadOnlyList<IApiFilterAttribute> FilterAttributes { get; protected set; }


        /// <summary>
        /// 获取Api的参数描述
        /// </summary>
        public abstract IReadOnlyList<ApiParameterDescriptor> Parameters { get; protected set; }

        /// <summary>
        /// 获取Api的返回描述
        /// </summary>
        public abstract ApiReturnDescriptor Return { get; protected set; }

        /// <summary>
        /// 获取自定义数据存储的字典
        /// </summary>
        public abstract ConcurrentDictionary<object, object> Properties { get; protected set; }
    }
}
