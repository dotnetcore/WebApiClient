using System.Threading.Tasks;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// 表示ApiActionInvoker提供者
    /// </summary>
    public class DefaultApiActionInvokerProvider : IApiActionInvokerProvider
    {
        /// <summary>
        /// 创建Action执行器
        /// </summary>
        /// <param name="actionDescriptor">Action描述</param>
        /// <returns></returns>
        public virtual ApiActionInvoker CreateActionInvoker(ApiActionDescriptor actionDescriptor)
        {
            var resultType = actionDescriptor.Return.DataType.Type;
            var invokerType = actionDescriptor.Return.ReturnType.IsInheritFrom<Task>()
                ? typeof(TaskApiActionInvoker<>).MakeGenericType(resultType)
                : typeof(ITaskApiActionInvoker<>).MakeGenericType(resultType);

            return invokerType.CreateInstance<ApiActionInvoker>(actionDescriptor);
        }
    }
}
