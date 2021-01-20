using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace WebApiClientCore.Internals.TypeProxies
{
    /// <summary>
    /// 提供接口方法查找
    /// </summary>
    static class HttpApiMethodFinder
    {
        /// <summary>
        /// 获取接口类型及其继承的接口的所有方法
        /// 忽略HttpApi类型的所有接口的方法
        /// </summary>
        /// <param name="interfaceType">接口类型</param> 
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        /// <returns></returns>
        public static MethodInfo[] FindApiMethods(Type interfaceType)
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
