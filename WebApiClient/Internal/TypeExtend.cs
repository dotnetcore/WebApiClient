using System;
using System.Collections.Concurrent;
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
        private static readonly ConcurrentCache<Type, MethodInfo[]> interfaceMethodsCache = new ConcurrentCache<Type, MethodInfo[]>();

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
            if (interfaceType.Detail().IsInterface == false)
            {
                throw new ArgumentException("类型必须为接口类型");
            }

            var apiMethods = new[] { interfaceType }.Concat(interfaceType.GetInterfaces())
                .Except(typeof(HttpApiClient).GetInterfaces())
                .SelectMany(item => item.GetMethods())
#if JIT
                .Select(item => item.EnsureApiMethod())
#endif
                .ToArray();

            return apiMethods;
        }

        /// <summary>
        /// 确保方法是支持的Api接口
        /// </summary>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        private static MethodInfo EnsureApiMethod(this MethodInfo method)
        {
            if (method.IsGenericMethod == true)
            {
                throw new NotSupportedException($"不支持泛型方法：{method}");
            }

            if (method.IsSpecialName == true)
            {
                throw new NotSupportedException($"不支持属性访问器：{method}");
            }

            var genericType = method.ReturnType;
            if (genericType.Detail().IsGenericType == true)
            {
                genericType = genericType.GetGenericTypeDefinition();
            }

            var isTaskType = genericType == typeof(Task<>) || genericType == typeof(ITask<>);
            if (isTaskType == false)
            {
                var message = $"返回类型必须为Task<>或ITask<>：{method}";
                throw new NotSupportedException(message);
            }

            foreach (var parameter in method.GetParameters())
            {
                if (parameter.ParameterType.IsByRef == true)
                {
                    var message = $"接口参数不支持ref/out修饰：{parameter}";
                    throw new NotSupportedException(message);
                }
            }

            return method;
        }

#if NETSTANDARD1_3
        /// <summary>
        /// 返回type的详细类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static TypeInfo Detail(this Type type)
        {
            return type.GetTypeInfo();
        }

        /// <summary>
        /// 获取构造参数
        /// </summary>
        /// <param name="typeInfo">类型</param>
        /// <param name="types">参数类型</param>
        /// <returns></returns>
        public static ConstructorInfo GetConstructor(this TypeInfo typeInfo, Type[] types)
        {
            return typeInfo
               .DeclaredConstructors
               .FirstOrDefault(item => item.GetParameters().Select(p => p.ParameterType).SequenceEqual(types));
        }
#else
        /// <summary>
        /// 返回type的详细类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type Detail(this Type type)
        {
            return type;
        }
#endif

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
            return typeAllowMultipleCache.GetOrAdd(type, (t => t.IsInheritFrom<Attribute>() && t.Detail().GetAttribute<AttributeUsageAttribute>(true).AllowMultiple));
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