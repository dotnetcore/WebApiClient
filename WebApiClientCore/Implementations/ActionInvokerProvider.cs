using System.Threading.Tasks;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// Action执行器提供者
    /// </summary>
    sealed class ActionInvokerProvider : IActionInvokerProvider
    {
        /// <summary>
        /// 获取静态实例
        /// </summary>
        public static IActionInvokerProvider Default { get; } = new ActionInvokerProvider();

        /// <summary>
        /// 创建Action执行器
        /// </summary>
        /// <param name="apiAction">Action描述</param>
        /// <returns></returns>
        public IActionInvoker CreateActionInvoker(ApiActionDescriptor apiAction)
        {
            var resultType = apiAction.Return.DataType.Type;
            var invokerType = apiAction.Return.ReturnType.IsInheritFrom<Task>()
                ? typeof(TaskActionInvoker<>).MakeGenericType(resultType)
                : typeof(ITaskActionInvoker<>).MakeGenericType(resultType);

            return invokerType.CreateInstance<IActionInvoker>(apiAction);
        }
    }
}
