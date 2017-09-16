using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 类型扩展
    /// </summary>
    static class TypeExtend
    {
        /// <summary>
        /// 获取是否为简单类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsSimple(this  Type type)
        {
            if (type.IsGenericType == true && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = Nullable.GetUnderlyingType(type);
            }          

            if (type.IsPrimitive || type.IsEnum)
            {
                return true;
            }

            return type == typeof(string)
                || type == typeof(decimal)
                || type == typeof(DateTime)
                || type == typeof(Guid)
                || type == typeof(Uri);
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
        /// 是否可以从baseType类型派生
        /// </summary>
        /// <param name="type"></param>
        /// <param name="baseType"></param>
        /// <returns></returns>
        public static bool IsInheritFrom(this Type type, Type baseType)
        {
            return baseType.IsAssignableFrom(type);
        }

        /// <summary>
        /// 是否实现了EnumerableOf(T)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsEnumerable<T>(this Type type)
        {
            if (type.IsArray && type.GetElementType().IsInheritFrom<T>())
            {
                return true;
            }

            if (type.IsInheritFrom<IEnumerable>() && type.IsGenericType && type.GetGenericArguments().FirstOrDefault().IsInheritFrom<T>())
            {
                return true;
            }
            return false;
        }
    }
}
