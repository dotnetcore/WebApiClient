namespace WebApiClientCore
{
    /// <summary>
    /// 表示Api参数上下文
    /// </summary>
    public class ApiParameterContext : ApiRequestContext
    {
        /// <summary>
        /// 获取参数描述
        /// </summary>
        public ApiParameterDescriptor Parameter { get; }

        /// <summary>
        /// 获取参数名
        /// </summary>
        public string ParameterName => this.Parameter.Name;

        /// <summary>
        /// 获取参数值
        /// </summary>
        public object? ParameterValue => this.Arguments[this.Parameter.Index];

        /// <summary>
        /// Api参数上下文
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameterIndex">参数索引</param>
        public ApiParameterContext(ApiRequestContext context, int parameterIndex, bool isIgnoreAutoUri)
            : this(context, context.ActionDescriptor.Parameters[parameterIndex], isIgnoreAutoUri)
        {
        }

        /// <summary>
        /// Api参数上下文
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="parameter">参数描述</param>
        public ApiParameterContext(ApiRequestContext context, ApiParameterDescriptor parameter, bool isIgnoreAutoUri)
            : base(context.HttpContext, context.ActionDescriptor, context.Arguments, context.Properties, isIgnoreAutoUri)
        {
            this.Parameter = parameter;
        }
    }
}
