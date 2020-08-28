using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
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
        /// 获取接口类型及其继承的接口的所有方法
        /// 忽略HttpApi类型的所有接口的方法
        /// </summary>
        /// <param name="interfaceType">接口类型</param> 
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public static MethodInfo[] GetAllApiMethods(this Type interfaceType)
        {
            if (interfaceType.IsInterface == false)
            {
                throw new ArgumentException(Resx.required_InterfaceType.Format(interfaceType.Name));
            }

            var apiMethods = interfaceType.GetInterfaces().Append(interfaceType)
                .SelectMany(item => item.GetMethods())
                .Select(item => item.EnsureApiMethod())
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
                throw new NotSupportedException(Resx.unsupported_GenericMethod.Format(method));
            }

            if (method.IsSpecialName == true)
            {
                throw new NotSupportedException(Resx.unsupported_Property.Format(method));
            }

            if (method.IsTaskReturn() == false)
            {
                var message = Resx.unsupported_ReturnType.Format(method);
                throw new NotSupportedException(message);
            }

            foreach (var parameter in method.GetParameters())
            {
                if (parameter.ParameterType.IsByRef == true)
                {
                    var message = Resx.unsupported_ByRef.Format(parameter);
                    throw new NotSupportedException(message);
                }
            }

            return method;
        }

        /// <summary>
        /// 检测方法是否为Task或ITask返回值
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private static bool IsTaskReturn(this MethodInfo method)
        {
            if (method.ReturnType.IsInheritFrom<Task>())
            {
                return true;
            }

            if (method.ReturnType.IsGenericType == false)
            {
                return false;
            }

            var taskType = method.ReturnType.GetGenericTypeDefinition();
            return taskType == typeof(ITask<>);
        }
    }
}