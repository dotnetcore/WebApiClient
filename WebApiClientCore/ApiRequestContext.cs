using System;
using System.Net.Http;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示Api请求的上下文
    /// </summary>
    public class ApiRequestContext
    {
        /// <summary>
        /// 获取http上下文
        /// </summary>
        public HttpContext HttpContext { get; }

        /// <summary>
        /// 获取关联的ApiAction描述
        /// </summary>
        public ApiActionDescriptor ApiAction { get; }

        /// <summary>
        /// 获取请求参数值
        /// </summary>
        public object?[] Arguments { get; }

        /// <summary>
        /// 获取自定义数据的存储和访问容器
        /// </summary>
        public DataCollection Properties { get; }

        /// <summary>
        /// 请求Api的上下文
        /// </summary> 
        /// <param name="httpContext"></param> 
        /// <param name="apiAction"></param>
        /// <param name="arguments"></param>
        public ApiRequestContext(HttpContext httpContext, ApiActionDescriptor apiAction, object?[] arguments)
            : this(httpContext, apiAction, arguments, new DataCollection())
        {
        }

        /// <summary>
        /// 请求Api的上下文
        /// </summary> 
        /// <param name="httpContext"></param> 
        /// <param name="apiAction"></param>
        /// <param name="arguments"></param>
        /// <param name="properties"></param> 
        protected ApiRequestContext(HttpContext httpContext, ApiActionDescriptor apiAction, object?[] arguments, DataCollection properties)
        {
            this.HttpContext = httpContext;
            this.ApiAction = apiAction;
            this.Arguments = arguments;
            this.Properties = properties;
        }

        /// <summary>
        /// 尝试根据参数名获取参数值
        /// </summary>
        /// <param name="parameterName">参数名</param> 
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool TryGetArgument(string parameterName, out object? value)
        {
            return this.TryGetArgument(parameterName, StringComparer.Ordinal, out value);
        }

        /// <summary>
        /// 尝试根据参数名获取参数值
        /// </summary>
        /// <param name="parameterName">参数名</param>
        /// <param name="comparer">比较器</param>
        /// <param name="value">值</param>
        /// <returns></returns>
        public bool TryGetArgument(string parameterName, StringComparer comparer, out object? value)
        {
            for (var i = 0; i < this.ApiAction.Parameters.Count; i++)
            {
                if (comparer.Equals(parameterName, this.ApiAction.Parameters[i].Name))
                {
                    value = this.Arguments[i];
                    return true;
                }
            }

            value = null;
            return false;
        }

        /// <summary>
        /// 返回请求使用的HttpCompletionOption
        /// </summary> 
        /// <returns></returns>
        public HttpCompletionOption GetCompletionOption()
        {
            if (this.HttpContext.CompletionOption != null)
            {
                return this.HttpContext.CompletionOption.Value;
            }

            var dataType = this.ApiAction.Return.DataType;
            return dataType.IsRawHttpResponseMessage || dataType.IsRawStream
                ? HttpCompletionOption.ResponseHeadersRead
                : HttpCompletionOption.ResponseContentRead;
        }
    }
}
