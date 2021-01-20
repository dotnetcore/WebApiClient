namespace WebApiClientCore
{
    /// <summary>
    /// 表示Action执行器
    /// </summary>
    public abstract class ApiActionInvoker
    {
        /// <summary>
        /// 获取Action描述
        /// </summary>
        public abstract ApiActionDescriptor ApiAction { get; }

        /// <summary>
        /// 执行Action
        /// </summary>
        /// <param name="context">服务上下文</param>
        /// <param name="arguments">action参数值</param>
        /// <returns></returns>
        public abstract object Invoke(HttpClientContext context, object?[] arguments);
    }
}
