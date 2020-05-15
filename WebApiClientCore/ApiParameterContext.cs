using System;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示Api参数上下文
    /// </summary>
    public class ApiParameterContext
    {
        /// <summary>
        /// 获取关联的Api上下文
        /// </summary>
        public ApiActionContext ApiActionContext { get; }

        /// <summary>
        /// 获取参数描述
        /// </summary>
        public ApiParameterDescriptor Parameter { get; }

        /// <summary>
        /// 获取参数值
        /// </summary>
        public object ParameterValue => this.ApiActionContext.Arguments[this.Parameter.Index];

        /// <summary>
        /// 获取关联的HttpRequestMessage
        /// </summary>
        public HttpApiRequestMessage RequestMessage => this.ApiActionContext.HttpContext.RequestMessage;

        /// <summary>
        /// 获取服务提供者
        /// </summary>
        public IServiceProvider RequestServices => this.ApiActionContext.HttpContext.RequestServices;

        /// <summary>
        /// Api参数上下文
        /// </summary>
        /// <param name="context"></param>
        /// <param name="parameter"></param>
        public ApiParameterContext(ApiActionContext context, ApiParameterDescriptor parameter)
        {
            this.ApiActionContext = context;
            this.Parameter = parameter;
        }
    }
}
