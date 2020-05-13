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
        /// <param name="parameter">参数</param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this ParameterInfo parameter, bool inherit) where TAttribute : class
        {
            return parameter.GetCustomAttributes(inherit).OfType<TAttribute>();
        }

        /// <summary>
        /// 从方法或声明的类型中查找第一个特性
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="method">方法</param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static TAttribute FindDeclaringAttribute<TAttribute>(this MethodInfo method, bool inherit) where TAttribute : class
        {
            return method.GetAttribute<TAttribute>(inherit) ?? method.DeclaringType.GetAttribute<TAttribute>(inherit);
        }

        /// <summary>
        /// 从方法和声明的类型中查找所有特性
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="method">方法</param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static IEnumerable<TAttribute> FindDeclaringAttributes<TAttribute>(this MethodInfo method, bool inherit) where TAttribute : class
        {
            var methodAttributes = method.GetAttributes<TAttribute>(inherit);
            var interfaceAttributes = method.DeclaringType.GetAttributes<TAttribute>(inherit);
            return methodAttributes.Concat(interfaceAttributes);
        }


        /// <summary>
        /// 获取成员的特性
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="member">成员</param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        public static TAttribute GetAttribute<TAttribute>(this MemberInfo member, bool inherit) where TAttribute : class
        {
            return member.GetAttributes<TAttribute>(inherit).FirstOrDefault();
        }

        /// <summary>
        /// 获取成员的特性
        /// </summary>
        /// <typeparam name="TAttribute"></typeparam>
        /// <param name="member">成员</param>
        /// <param name="inherit"></param>
        /// <returns></returns>
        private static IEnumerable<TAttribute> GetAttributes<TAttribute>(this MemberInfo member, bool inherit) where TAttribute : class
        {
            return member.GetCustomAttributes(inherit).OfType<TAttribute>();
        }
    }
}
