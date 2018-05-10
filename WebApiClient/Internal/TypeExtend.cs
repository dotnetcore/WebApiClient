using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebApiClient.DataAnnotations;

namespace WebApiClient
{
    /// <summary>
    /// 类型扩展
    /// </summary>
    static class TypeExtend
    {
        /// <summary>
        /// 类型是否AllowMultiple的缓存
        /// </summary>
        private static readonly ConcurrentDictionary<Type, bool> typeAllowMultipleCache = new ConcurrentDictionary<Type, bool>();

       
        /// <summary>
        /// 是否可以从TBase类型派生
        /// </summary>
        /// <typeparam name="TBase"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsInheritFrom<TBase>(this Type type)
        {
            return typeof(TBase).IsAssignableFrom(type);
        }

        /// <summary>
        /// 关联的AttributeUsageAttribute是否AllowMultiple
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsAllowMultiple(this Type type)
        {
            return typeAllowMultipleCache.GetOrAdd(type, (t => t.IsInheritFrom<Attribute>() && t.GetAttribute<AttributeUsageAttribute>(true).AllowMultiple));
        }

        /// <summary>
        /// 返回特性是否声明指定的FormatScope
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public static bool IsDefinedFormatScope<T>(this MemberInfo element, FormatScope scope) where T : DataAnnotationAttribute
        {
            var attribute = element.GetCustomAttribute<T>();
            return attribute != null && attribute.IsDefinedScope(scope);
        }
    }
}
