using System;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
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
        private static readonly ConcurrentDictionary<Type, object?> defaultValueCache = new();

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
        public static T CreateInstance<T>(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]
            this Type type, 
            params object?[] args)
        {
            var instance = Activator.CreateInstance(type, args);
            return instance == null ? throw new TypeInstanceCreateException(type) : (T)instance;
        }

        /// <summary>
        /// 获取接口和其基础接口的特性
        /// </summary> 
        /// <param name="interfaceType">接口类型</param> 
        /// <returns></returns>
        public static Attribute[] GetInterfaceCustomAttributes(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
            this Type interfaceType)
        {
            return interfaceType
                .GetInterfaces()
                .Prepend(interfaceType)
                .SelectMany(item => item.GetCustomAttributes())
                .ToArray();
        }
    }
}