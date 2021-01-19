using WebApiClientCore.Abstractions;

namespace WebApiClientCore.Implementations
{
    /// <summary>
    /// 表示ITask返回声明的Action执行器
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    class ITaskActionInvoker<TResult> : IActionInvoker
    {
        /// <summary>
        /// Api执行器
        /// </summary>
        private readonly TaskActionInvoker<TResult> actionInvoker;

        /// <summary>
        /// 获取Action描述
        /// </summary>
        public ApiActionDescriptor ApiAction => this.actionInvoker.ApiAction;

        /// <summary>
        /// ITask返回声明的Action执行器
        /// </summary>
        /// <param name="apiAction">Api描述</param>
        public ITaskActionInvoker(ApiActionDescriptor apiAction)
        {
            this.actionInvoker = new TaskActionInvoker<TResult>(apiAction);
        }

        /// <summary>
        /// 执行Action
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="arguments">参数值</param>
        /// <returns></returns>
        public object Invoke(HttpClientContext context, object?[] arguments)
        {
            return new ActionTask<TResult>(this.actionInvoker, context, arguments);
        }
    }
}