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
        /// 接口的方法缓存
        /// </summary>
        private static readonly ConcurrentDictionary<Type, MethodInfo[]> interfaceMethodsCache = new ConcurrentDictionary<Type, MethodInfo[]>();

        /// <summary>
        /// 类型是否AllowMultiple的缓存
        /// </summary>
        private static readonly ConcurrentDictionary<Type, bool> typeAllowMultipleCache = new ConcurrentDictionary<Type, bool>();


        /// <summary>
        /// 获取接口类型及其继承的接口的所有方法
        /// 忽略HttpApiClient类型的所有接口的方法
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public static MethodInfo[] GetAllApiMethods(this Type interfaceType)
        {
            return interfaceMethodsCache.GetOrAdd(
                interfaceType,
                type => type.GetAllApiMethodsNoCache());
        }

        /// <summary>
        /// 获取接口类型及其继承的接口的所有方法
        /// 忽略HttpApiClient类型的所有接口的方法
        /// </summary>
        /// <param name="interfaceType">接口类型</param> 
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        private static MethodInfo[] GetAllApiMethodsNoCache(this Type interfaceType)
        {
            if (interfaceType.IsInterface == false)
            {
                throw new ArgumentException("类型必须为接口类型");
            }

            // 接口的实现在动态程序集里，所以接口必须为public修饰才可以创建代理类并实现此接口
            var attrs = new[] { TypeAttributes.Public, TypeAttributes.NestedPublic };
            var attr = attrs.Aggregate((a, b) => a | b) & interfaceType.Attributes;
            if (attrs.Contains(attr) == false)
            {
                throw new NotSupportedException(interfaceType.Name + "必须为public修饰");
            }

            // 排除HttpApiClient已实现的接口
            var excepts = typeof(HttpApiClient).GetInterfaces();
            var exceptHashSet = new HashSet<Type>(excepts);
            var methodHashSet = new HashSet<MethodInfo>();

            interfaceType.GetInterfaceMethods(ref exceptHashSet, ref methodHashSet);
            return methodHashSet.ToArray();
        }

        /// <summary>
        /// 递归查找接口的方法
        /// </summary>
        /// <param name="interfaceType">接口类型</param>
        /// <param name="exceptHashSet">排除的接口类型</param>
        /// <param name="methodHashSet">收集到的方法</param>
        /// <exception cref="NotSupportedException"></exception>
        private static void GetInterfaceMethods(this Type interfaceType, ref HashSet<Type> exceptHashSet, ref HashSet<MethodInfo> methodHashSet)
        {
            if (exceptHashSet.Add(interfaceType) == false)
            {
                return;
            }

            var methods = interfaceType.GetMethods();
            foreach (var item in methods)
            {
                item.EnsureApiMethod();
                methodHashSet.Add(item);
            }

            foreach (var item in interfaceType.GetInterfaces())
            {
                item.GetInterfaceMethods(ref exceptHashSet, ref methodHashSet);
            }
        }

        /// <summary>
        /// 确保方法是支持的Api接口
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        private static void EnsureApiMethod(this MethodInfo method)
        {
            if (method.IsGenericMethod == true)
            {
                throw new NotSupportedException("不支持泛型方法：" + method);
            }

            if (method.IsSpecialName == true)
            {
                throw new NotSupportedException("不支持属性访问器：" + method);
            }

            var genericType = method.ReturnType;
            if (genericType.IsGenericType == true)
            {
                genericType = genericType.GetGenericTypeDefinition();
            }

            var isTaskType = genericType == typeof(Task<>) || genericType == typeof(ITask<>);
            if (isTaskType == false)
            {
                var message = string.Format("接口{0}返回类型必须为Task<>或ITask<>", method.Name);
                throw new NotSupportedException(message);
            }

            foreach (var parameter in method.GetParameters())
            {
                if (parameter.ParameterType.IsByRef == true)
                {
                    var message = string.Format("接口参数不支持ref/out修饰：{0}", parameter);
                    throw new NotSupportedException(message);
                }
            }
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

        /// <summary>
        /// 返回特性是否声明指定的Scope
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public static bool IsDefinedAnnotateScope<T>(this MemberInfo element, FormatScope scope) where T : DataAnnotationAttribute
        {
            var attribute = element.GetCustomAttribute<T>();
            return attribute != null && attribute.IsDefinedScope(scope);
        }
    }
}
