using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore
{
    /// <summary>
    /// 类型扩展
    /// </summary>
    static class TypeExtensions
    {
        /// <summary>
        /// 类型的默认值缓存
        /// </summary>
        private static readonly ConcurrentDictionary<Type, object?> defaultValueCache = new ConcurrentDictionary<Type, object?>();

        /// <summary>
        /// 关联的AttributeUsageAttribute是否AllowMultiple
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsAllowMultiple(this Type type)
        {
            return type.GetCustomAttribute<AttributeUsageAttribute>()?.AllowMultiple == true;
        }

        /// <summary>
        /// 返回类型的默认值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object? DefaultValue(this Type? type)
        {
            return type == null ? null : defaultValueCache.GetOrAdd(type, t =>
            {
                var value = Expression.Convert(Expression.Default(t), typeof(object));
                return Expression.Lambda<Func<object>>(value).Compile().Invoke();
            });
        }

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
        /// 创建实例
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type">实例类型</param>
        /// <param name="args">参数值</param>
        /// <exception cref="TypeInstanceCreateException"></exception>
        /// <returns></returns>
        public static T CreateInstance<T>(this Type type, params object?[] args)
        {
            var instance = Activator.CreateInstance(type, args);
            if (instance == null)
            {
                throw new TypeInstanceCreateException(type);
            }
            return (T)instance;
        }

        /// <summary>
        /// 获取自定义特性
        /// </summary> 
        /// <param name="interfaceType">接口类型</param>
        /// <param name="inclueBases">是否包括基础接口定义的特性</param> 
        /// <returns></returns>
        public static IEnumerable<Attribute> GetInterfaceCustomAttributes(this Type interfaceType, bool inclueBases = true)
        {
            var types = Enumerable.Repeat(interfaceType, 1);
            if (inclueBases == true)
            {
                types = types.Concat(interfaceType.GetInterfaces());
            }
            return types.SelectMany(item => item.GetCustomAttributes()).ToArray();
        }
    }
}