using WebApiClientCore.Implementations.Tasks;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// 表示ITask返回声明的Action执行器
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    sealed class ITaskApiActionInvoker<TResult> : ApiActionInvoker
    {
        /// <summary>
        /// Api执行器
        /// </summary>
        private readonly TaskApiActionInvoker<TResult> actionInvoker;

        /// <summary>
        /// 获取Action描述
        /// </summary>
        public override ApiActionDescriptor ActionDescriptor => this.actionInvoker.ActionDescriptor;

        /// <summary>
        /// ITask返回声明的Action执行器
        /// </summary>
        /// <param name="actionDescriptor">Api描述</param>
        public ITaskApiActionInvoker(ApiActionDescriptor actionDescriptor)
        {
            this.actionInvoker = new TaskApiActionInvoker<TResult>(actionDescriptor);
        }

        /// <summary>
        /// 执行Action
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="arguments">参数值</param>
        /// <returns></returns>
        public override object Invoke(HttpClientContext context, object?[] arguments)
        {
            return new ActionTask<TResult>(this.actionInvoker, context, arguments);
        }
    }
}