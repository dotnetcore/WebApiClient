using System;
using System.ComponentModel;

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
        public ApiActionDescriptor ActionDescriptor { get; }

        /// <summary>
        /// 获取关联的ApiAction描述 
        /// </summary>
        [Obsolete("请使用ActionDescriptor属性")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ApiActionDescriptor ApiAction => this.ActionDescriptor;

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
        /// <param name="actionDescriptor"></param>
        /// <param name="arguments"></param>
        public ApiRequestContext(HttpContext httpContext, ApiActionDescriptor actionDescriptor, object?[] arguments)
            : this(httpContext, actionDescriptor, arguments, new DataCollection())
        {
        }

        /// <summary>
        /// 请求Api的上下文
        /// </summary> 
        /// <param name="httpContext"></param> 
        /// <param name="actionDescriptor"></param>
        /// <param name="arguments"></param>
        /// <param name="properties"></param> 
        protected ApiRequestContext(HttpContext httpContext, ApiActionDescriptor actionDescriptor, object?[] arguments, DataCollection properties)
        {
            this.HttpContext = httpContext;
            this.ActionDescriptor = actionDescriptor;
            this.Arguments = arguments;
            this.Properties = properties;
        }
    }
}
