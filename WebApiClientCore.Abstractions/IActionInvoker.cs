namespace WebApiClientCore
{
    /// <summary>
    /// 定义Action执行器的接口
    /// </summary>
    public interface IActionInvoker
    {
        /// <summary>
        /// 获取Action描述
        /// </summary>
        ApiActionDescriptor ApiAction { get; }

        /// <summary>
        /// 执行Action
        /// </summary>
        /// <param name="context">服务上下文</param>
        /// <param name="arguments">action参数值</param>
        /// <returns></returns>
        object Invoke(HttpClientContext context, object?[] arguments);
    }
}
