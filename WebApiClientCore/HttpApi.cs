using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WebApiClientCore.Internals;

namespace WebApiClientCore
{
    /// <summary>
    /// 提供HttpApi命名获取和方法获取等功能
    /// </summary>
    public static class HttpApi
    {
        /// <summary>
        /// 获取接口的名称
        /// 该名称可用于接口对应的OptionsName
        /// </summary>
        /// <param name="httpApiType">接口类型</param>
        /// <returns></returns>
        public static string GetName(Type? httpApiType)
        {
            return GetName(httpApiType, includeNamespace: true);
        }

        /// <summary>
        /// 获取接口的名称 
        /// </summary>
        /// <param name="httpApiType">接口类型</param>
        /// <param name="includeNamespace">是否包含命名空间</param>
        /// <returns></returns>
        public static string GetName(Type? httpApiType, bool includeNamespace)
        {
            if (httpApiType == null)
            {
                return string.Empty;
            }

            var builder = new ValueStringBuilder(stackalloc char[256]);
            if (includeNamespace == true)
            {
                builder.Append(httpApiType.Namespace);
                builder.Append(".");
            }

            GetName(httpApiType, ref builder);
            return builder.ToString();
        }


        /// <summary>
        /// 获取类型的短名称
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="builder"></param>
        /// <returns></returns>
        private static void GetName(Type type, ref ValueStringBuilder builder)
        {
            if (type.IsGenericType == false)
            {
                builder.Append(type.Name);
                return;
            }

            var name = type.Name.AsSpan();
            var index = name.LastIndexOf('`');
            if (index > -1)
            {
                name = name[..index];
            }
            builder.Append(name);
            builder.Append('<');

            var i = 0;
            var arguments = type.GetGenericArguments();
            foreach (var argument in arguments)
            {
                GetName(argument, ref builder);
                if (++i < arguments.Length)
                {
                    builder.Append(',');
                }
            }
            builder.Append('>');
        }


        /// <summary>
        /// 查找接口类型及其继承的接口的所有方法
        /// </summary>
        /// <param name="httpApiType">接口类型</param> 
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public static MethodInfo[] FindApiMethods(Type httpApiType)
        {
            if (httpApiType.IsInterface == false)
            {
                throw new ArgumentException(Resx.required_InterfaceType.Format(httpApiType.Name));
            }

            return httpApiType.GetInterfaces().Append(httpApiType)
                .SelectMany(item => item.GetMethods())
                .Select(item => item.EnsureApiMethod())
                .ToArray();
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
