namespace WebApiClientCore
{
    /// <summary>
    /// Action执行器提供者
    /// </summary>
    class ActionInvokerProvider : IActionInvokerProvider
    {
        /// <summary>
        /// 获取静态实例
        /// </summary>
        public static IActionInvokerProvider Instance { get; } = new ActionInvokerProvider();

        /// <summary>
        /// 创建Action执行器
        /// </summary>
        /// <param name="apiAction">Action描述</param>
        /// <returns></returns>
        public IActionInvoker CreateActionInvoker(ApiActionDescriptor apiAction)
        {
            var resultType = apiAction.Return.DataType.Type;
            var invokerType = typeof(MultiplexedActionInvoker<>).MakeGenericType(resultType);
            return invokerType.CreateInstance<IActionInvoker>(apiAction);
        }
    }
}
