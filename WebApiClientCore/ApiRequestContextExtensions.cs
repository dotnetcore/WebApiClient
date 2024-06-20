using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace WebApiClientCore
{
    /// <summary>
    /// 提供ApiRequestContext的扩展
    /// </summary>
    public static class ApiRequestContextExtensions
    {
        /// <summary>
        /// 尝试根据参数名获取参数值
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="context"></param>
        /// <param name="parameterName">参数名</param> 
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool TryGetArgument<TValue>(this ApiRequestContext context, string parameterName, [MaybeNullWhen(false)] out TValue value)
        {
            return context.TryGetArgument(parameterName, StringComparer.Ordinal, out value);
        }

        /// <summary>
        /// 尝试根据参数名获取参数值
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="context"></param>
        /// <param name="parameterName">参数名</param>
        /// <param name="nameComparer">比较器</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool TryGetArgument<TValue>(this ApiRequestContext context, string parameterName, StringComparer nameComparer, [MaybeNullWhen(false)] out TValue value)
        {
            if (context.TryGetArgument(parameterName, nameComparer, out var objValue) && objValue is TValue tValue)
            {
                value = tValue;
                return true;
            }

            value = default;
            return false;
        }

        /// <summary>
        /// 尝试根据参数名获取参数值
        /// </summary>
        /// <param name="context"></param>
        /// <param name="parameterName">参数名</param> 
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool TryGetArgument(this ApiRequestContext context, string parameterName, out object? value)
        {
            return context.TryGetArgument(parameterName, StringComparer.Ordinal, out value);
        }

        /// <summary>
        /// 尝试根据参数名获取参数值
        /// </summary>
        /// <param name="context"></param>
        /// <param name="parameterName">参数名</param>
        /// <param name="nameComparer">比较器</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool TryGetArgument(this ApiRequestContext context, string parameterName, StringComparer nameComparer, out object? value)
        {
            foreach (var parameter in context.ActionDescriptor.Parameters)
            {
                if (nameComparer.Equals(parameter.Name, parameterName))
                {
                    value = context.Arguments[parameter.Index];
                    return true;
                }
            }

            value = null;
            return false;
        }

        /// <summary>
        /// 获取以 ApiAction 为容器的 ILogger
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static ILogger? GetActionLogger(this ApiRequestContext context)
        {
            return context.ActionDescriptor.Properties.GetOrAdd(typeof(ILogger), CreateLogger) as ILogger;

            object? CreateLogger(object _)
            {
                var loggerFactory = context.HttpContext.ServiceProvider.GetService<ILoggerFactory>();
                if (loggerFactory == null)
                {
                    return null;
                }

                var action = context.ActionDescriptor;
                var parameters = action.Parameters.Select(item => HttpApi.GetName(item.ParameterType, includeNamespace: false));
                var categoryName = $"{action.InterfaceType.FullName}.{action.Member.Name}({string.Join(", ", parameters)})";
                return loggerFactory.CreateLogger(categoryName);
            }
        }
    }
}
