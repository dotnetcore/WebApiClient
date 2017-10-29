using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
        /// void类型
        /// </summary>
        private static readonly Type voidType = typeof(void);

        /// <summary>
        /// dispose方法
        /// </summary>
        private static readonly MethodInfo disposeMethod = typeof(IDisposable).GetMethods().FirstOrDefault();

        /// <summary>
        /// 接口的方法缓存
        /// </summary>
        private static readonly ConcurrentDictionary<Type, MethodInfo[]> interfaceMethodsCache = new ConcurrentDictionary<Type, MethodInfo[]>();

        /// <summary>
        /// 类型是否AllowMultiple的缓存
        /// </summary>
        private static readonly ConcurrentDictionary<Type, bool> typeAllowMultipleCache = new ConcurrentDictionary<Type, bool>();

        /// <summary>
        /// 确保类型是Api接口
        /// </summary>
        /// <param name="apiType">接口类型</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public static void EnsureApiInterface(this Type apiType)
        {
            if (apiType.IsInterface == false)
            {
                throw new ArgumentException(apiType.Name + "不是接口类型");
            }

            // 接口的实现在动态程序集里，所以接口必须为public修饰才可以创建代理类并实现此接口
            if (TypeAttributes.Public != (TypeAttributes.Public & apiType.Attributes))
            {
                throw new NotSupportedException(apiType.Name + "必须为public修饰");
            }

            foreach (var m in apiType.GetInterfaceAllMethods())
            {
                if (m.Equals(disposeMethod) == true)
                {
                    continue;
                }

                if (m.IsGenericMethod == true)
                {
                    throw new NotSupportedException("不支持泛型方法：" + m);
                }
            }
        }

        /// <summary>
        /// 获取接口类型及其继承的接口的所有方法
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <returns></returns>
        public static MethodInfo[] GetInterfaceAllMethods(this Type interfaceType)
        {
            if (interfaceType.IsInterface == false)
            {
                throw new NotSupportedException("类型必须为接口类型");
            }

            return interfaceMethodsCache.GetOrAdd(interfaceType, type =>
            {
                var typeHashSet = new HashSet<Type>();
                var methodHashSet = new HashSet<MethodInfo>();
                SearchInterfaceMethods(type, ref typeHashSet, ref methodHashSet);
                return methodHashSet.ToArray();
            });
        }

        /// <summary>
        /// 递归查找接口的方法
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <param name="typeHashSet">接口类型集</param>
        /// <param name="methodHashSet">方法集</param>
        private static void SearchInterfaceMethods(Type interfaceType, ref HashSet<Type> typeHashSet, ref HashSet<MethodInfo> methodHashSet)
        {
            if (typeHashSet.Add(interfaceType) == true)
            {
                var methods = interfaceType.GetMethods();
                foreach (var item in methods)
                {
                    methodHashSet.Add(item);
                }

                foreach (var item in interfaceType.GetInterfaces())
                {
                    SearchInterfaceMethods(item, ref typeHashSet, ref methodHashSet);
                }
            }
        }

        /// <summary>
        /// 获取是否为简单类型
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static bool IsSimple(this Type type)
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
        /// 关联的AttributeUsageAttribute是否AllowMultiple
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool AllowMultiple(this Type type)
        {
            return typeAllowMultipleCache.GetOrAdd(type, (t => t.IsInheritFrom<Attribute>() && t.GetAttribute<AttributeUsageAttribute>(true).AllowMultiple));
        }
    }
}
