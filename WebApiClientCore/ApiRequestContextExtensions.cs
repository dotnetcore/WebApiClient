using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;

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
        /// <typeparam name="TValue"></typeparam>
        /// <param name="context"></param>
        /// <param name="parameterName">参数名</param> 
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool TryGetArgument<TValue>(this ApiRequestContext context, string parameterName, [MaybeNull] out TValue value)
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
        public static bool TryGetArgument<TValue>(this ApiRequestContext context, string parameterName, StringComparer nameComparer, [MaybeNull] out TValue value)
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
        /// <param name="nameComparer">比较器</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public static bool TryGetArgument(this ApiRequestContext context, string parameterName, StringComparer nameComparer, out object? value)
        {
            foreach (var parameter in context.ApiAction.Parameters)
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
        /// 返回请求使用的HttpCompletionOption
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static HttpCompletionOption GetCompletionOption(this ApiRequestContext context)
        {
            if (context.HttpContext.CompletionOption != null)
            {
                return context.HttpContext.CompletionOption.Value;
            }

            var dataType = context.ApiAction.Return.DataType;
            return dataType.IsRawHttpResponseMessage || dataType.IsRawStream
                ? HttpCompletionOption.ResponseHeadersRead
                : HttpCompletionOption.ResponseContentRead;
        }
    }
}
