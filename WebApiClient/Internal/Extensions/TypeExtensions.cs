using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 类型扩展
    /// </summary>
    static class TypeExtensions
    {
        /// <summary>
        /// 类型是否AllowMultiple的缓存
        /// </summary>
        private static readonly ConcurrentCache<Type, bool> typeAllowMultipleCache = new ConcurrentCache<Type, bool>();

        /// <summary>
        /// 类型的默认值缓存
        /// </summary>
        private static readonly ConcurrentCache<Type, object> typeDefaultValueCache = new ConcurrentCache<Type, object>();

        /// <summary>
        /// 表示0个元素的类型集合
        /// </summary>
        public static readonly Type[] EmptyTypes = new Type[0];

        /// <summary>
        /// 关联的AttributeUsageAttribute是否AllowMultiple
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsAllowMultiple(this Type type)
        {
            return typeAllowMultipleCache.GetOrAdd(type, (t => t.IsInheritFrom<Attribute>() && t.GetTypeInfo().GetCustomAttribute<AttributeUsageAttribute>(true).AllowMultiple));
        }

        /// <summary>
        /// 返回类型的默认值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static object DefaultValue(this Type type)
        {
            if (type == null)
            {
                return null;
            }

            return typeDefaultValueCache.GetOrAdd(type, t =>
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


#if !NETSTANDARD1_3        
        /// <summary>
        /// 返回type的详细类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type GetTypeInfo(this Type type)
        {
            return type;
        }

        /// <summary>
        /// 转换为Type类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Type AsType(this Type type)
        {
            return type;
        }
#endif

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
            if (interfaceType.GetTypeInfo().IsInterface == false)
            {
                throw new ArgumentException("类型必须为接口类型");
            }

            var apiMethods = new[] { interfaceType }.Concat(interfaceType.GetInterfaces())
                .Except(typeof(HttpApiClient).GetInterfaces())
                .SelectMany(item => item.GetMethods())
#if JIT
                .Select(item => item.EnsureApiMethod())
#endif
                .OrderBy(item => item.GetFullName())
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
            if (genericType.GetTypeInfo().IsGenericType == true)
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

        /// <summary>
        /// 返回方法的完整名称
        /// </summary>
        /// <param name="method">方法</param>
        /// <returns></returns>
        private static string GetFullName(this MethodInfo method)
        {
            var builder = new StringBuilder();
            foreach (var p in method.GetParameters())
            {
                if (builder.Length > 0)
                {
                    builder.Append(",");
                }
                builder.Append(p.ParameterType.GetName());
            }

            var insert = $"{method.ReturnType.GetName()} {method.Name}(";
            return builder.Insert(0, insert).Append(")").ToString();
        }

        /// <summary>
        /// 返回类型不含namespace的名称
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns></returns>
        private static string GetName(this Type type)
        {
            if (type.GetTypeInfo().IsGenericType == false)
            {
                return type.Name;
            }

            var builder = new StringBuilder();
            foreach (var argType in type.GetGenericArguments())
            {
                if (builder.Length > 0)
                {
                    builder.Append(",");
                }
                builder.Append(argType.GetName());
            }

            return builder.Insert(0, $"{type.Name}<").Append(">").ToString();
        }
    }
}