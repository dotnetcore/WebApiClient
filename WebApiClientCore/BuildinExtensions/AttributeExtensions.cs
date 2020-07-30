using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace WebApiClientCore
{
    /// <summary>
    /// 成员特性扩展
    /// </summary>
    static class AttributeExtensions
    {
        /// <summary>
        /// 获取方法的特性
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="method">方法</param> 
        /// <returns></returns>
        public static TAttribute? GetAttribute<TAttribute>(this MethodInfo method) where TAttribute : class
        {
            return method.GetAttributes<TAttribute>().FirstOrDefault();
        }

        /// <summary>
        /// 获取方法的特性
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="method">方法</param> 
        /// <returns></returns>
        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this MethodInfo method) where TAttribute : class
        {
            return method.GetCustomAttributes().OfType<TAttribute>();
        }

        /// <summary>
        /// 获取参数的特性
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="parameter">参数</param> 
        /// <returns></returns>
        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this ParameterInfo parameter) where TAttribute : class
        {
            return parameter.GetCustomAttributes().OfType<TAttribute>();
        }

        /// <summary>
        /// 获取接口以及其基础接口定义的特性
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="interfaceType">接口类型</param> 
        /// <returns></returns>
        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this Type interfaceType) where TAttribute : class
        {
            return new[] { interfaceType }
                .Concat(interfaceType.GetInterfaces())
                .SelectMany(item => item.GetCustomAttributes()
                .OfType<TAttribute>());
        }
    }
}
