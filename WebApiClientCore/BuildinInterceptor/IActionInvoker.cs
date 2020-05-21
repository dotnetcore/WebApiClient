namespace WebApiClientCore
{
    /// <summary>
    /// 定义ApiAction执行器的接口
    /// </summary>
    interface IActionInvoker
    {
        /// <summary>
        /// 执行任务
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="arguments">参数值</param>
        /// <returns></returns>
        object Invoke(ServiceContext context, object?[] arguments);
    }
}
