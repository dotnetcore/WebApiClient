namespace WebApiClientCore
{
    /// <summary>
    /// 表示Api参数上下文
    /// </summary>
    public class ApiParameterContext
    {
        /// <summary>
        /// 获取请求上下文
        /// </summary>
        public ApiRequestContext RequestContext { get; }

        /// <summary>
        /// 获取http上下文
        /// </summary>
        public HttpContext HttpContext => this.RequestContext.HttpContext;

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
        public ApiParameterContext(ApiRequestContext context, int parameterIndex)
        {
            this.RequestContext = context;
            this.Parameter = this.RequestContext.ApiAction.Parameters[parameterIndex];
            this.ParameterValue = this.RequestContext.Arguments[parameterIndex];
        }

        /// <summary>
        /// Api参数上下文
        /// for test
        /// </summary>
        /// <param name="context"></param>
        /// <param name="parameter"></param>
        /// <param name="parameterValue"></param>
        internal ApiParameterContext(ApiRequestContext context, ApiParameterDescriptor parameter, object parameterValue)
        {
            this.RequestContext = context;
            this.Parameter = parameter;
            this.ParameterValue = parameterValue;
        }
    }
}
