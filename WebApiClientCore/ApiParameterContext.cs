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
        public ApiActionContext ActionContext { get; }

        /// <summary>
        /// 获取http上下文
        /// </summary>
        public HttpContext HttpContext => this.ActionContext.HttpContext;

        /// <summary>
        /// 获取参数描述
        /// </summary>
        public ApiParameterDescriptor Parameter { get; }

        /// <summary>
        /// 获取参数值
        /// </summary>
        public object ParameterValue { get; }

        /// <summary>
        /// Api参数上下文
        /// </summary>
        /// <param name="context"></param>
        /// <param name="parameterIndex"></param>
        public ApiParameterContext(ApiActionContext context, int parameterIndex)
        {
            this.ActionContext = context;
            this.Parameter = this.ActionContext.ApiAction.Parameters[parameterIndex];
            this.ParameterValue = this.ActionContext.Arguments[parameterIndex];
        }

        /// <summary>
        /// Api参数上下文
        /// </summary>
        /// <param name="context"></param>
        /// <param name="parameter"></param>
        /// <param name="parameterValue"></param>
        public ApiParameterContext(ApiActionContext context, ApiParameterDescriptor parameter, object parameterValue)
        {
            this.ActionContext = context;
            this.Parameter = parameter;
            this.ParameterValue = parameterValue;
        }
    }
}
