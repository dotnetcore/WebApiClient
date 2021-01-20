using System.Threading.Tasks;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// ActionInvoker提供者的接口
    /// </summary>
    public class DefaultActionInvokerProvider : IActionInvokerProvider
    {
        /// <summary>
        /// 创建Action执行器
        /// </summary>
        /// <param name="apiAction">Action描述</param>
        /// <returns></returns>
        public virtual IActionInvoker CreateActionInvoker(ApiActionDescriptor apiAction)
        {
            var resultType = apiAction.Return.DataType.Type;
            var invokerType = apiAction.Return.ReturnType.IsInheritFrom<Task>()
                ? typeof(TaskActionInvoker<>).MakeGenericType(resultType)
                : typeof(ITaskActionInvoker<>).MakeGenericType(resultType);

            return invokerType.CreateInstance<IActionInvoker>(apiAction);
        }
    }
}
