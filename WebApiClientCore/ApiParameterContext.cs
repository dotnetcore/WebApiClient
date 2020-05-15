namespace WebApiClientCore
{
    /// <summary>
    /// 表示Api参数上下文
    /// </summary>
    public class ApiParameterContext : ApiRequestContext
    {
        /// <summary>
        /// 参数索引
        /// </summary>
        private readonly int index;

        /// <summary>
        /// 获取参数描述
        /// </summary>
        public ApiParameterDescriptor Parameter => this.ApiAction.Parameters[index];

        /// <summary>
        /// 获取参数值
        /// </summary>
        public object ParameterValue => this.Arguments[index];

        /// <summary>
        /// Api参数上下文
        /// </summary>
        /// <param name="context"></param>
        /// <param name="parameterIndex"></param>
        public ApiParameterContext(ApiRequestContext context, int parameterIndex)
            : base(context.HttpContext, context.ApiAction, context.Arguments)
        {
            this.index = parameterIndex;
            this.Tags = context.Tags;
            this.CancellationTokens = context.CancellationTokens;
        }
    }
}
