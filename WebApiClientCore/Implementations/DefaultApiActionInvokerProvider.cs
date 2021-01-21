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
            var invokerType = typeof(DefaultApiActionInvoker<>).MakeGenericType(resultType);
            var actionInvoker = invokerType.CreateInstance<ApiActionInvoker>(actionDescriptor);

            if (actionDescriptor.Return.ReturnType.IsInheritFrom<Task>() == false)
            {
                var convertable = (IITaskReturnConvertable)actionInvoker;
                actionInvoker = convertable.ToITaskReturnActionInvoker();
            }
            return actionInvoker;
        }
    }
}
