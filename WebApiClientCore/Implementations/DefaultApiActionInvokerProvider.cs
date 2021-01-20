using System.Threading.Tasks;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// ActionInvoker提供者的接口
    /// </summary>
    public class DefaultApiActionInvokerProvider : IApiActionInvokerProvider
    {
        /// <summary>
        /// 创建Action执行器
        /// </summary>
        /// <param name="apiAction">Action描述</param>
        /// <returns></returns>
        public virtual ApiActionInvoker CreateActionInvoker(ApiActionDescriptor apiAction)
        {
            var resultType = apiAction.Return.DataType.Type;
            var invokerType = apiAction.Return.ReturnType.IsInheritFrom<Task>()
                ? typeof(TaskApiActionInvoker<>).MakeGenericType(resultType)
                : typeof(ITaskApiActionInvoker<>).MakeGenericType(resultType);

            return invokerType.CreateInstance<ApiActionInvoker>(apiAction);
        }
    }
}
