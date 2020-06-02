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
        /// 获取成员的特性
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="member">成员</param> 
        /// <returns></returns>
        public static TAttribute? GetAttribute<TAttribute>(this MemberInfo member) where TAttribute : class
        {
            return member.GetCustomAttributes().OfType<TAttribute>().FirstOrDefault();
        }

        /// <summary>
        /// 获取成员的特性
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="member">成员</param>
        /// <param name="includeDeclaringType">是否也包括声明类型</param>
        /// <returns></returns>
        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this MemberInfo member, bool includeDeclaringType) where TAttribute : class
        {
            var self = member.GetCustomAttributes().OfType<TAttribute>();
            if (includeDeclaringType == false || member.DeclaringType == null)
            {
                return self;
            }

            var decalring = member.DeclaringType.GetCustomAttributes().OfType<TAttribute>();
            return self.Concat(decalring);
        }

        /// <summary>
        /// 获取成员的特性
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="parameter">参数</param> 
        /// <returns></returns>
        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this ParameterInfo parameter) where TAttribute : class
        {
            return parameter.GetCustomAttributes().OfType<TAttribute>();
        }
    }
}
