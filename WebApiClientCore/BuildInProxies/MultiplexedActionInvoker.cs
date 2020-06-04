using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示复合的ApiAction执行器
    /// 支持Task和ITask返回声明
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    class MultiplexedActionInvoker<TResult> : IActionInvoker
    {
        /// <summary>
        /// 是否为task结果声明
        /// </summary>
        private readonly bool isTaskResult;

        /// <summary>
        /// Api执行器
        /// </summary>
        private readonly ActionInvoker<TResult> actionInvoker;

        /// <summary>
        /// 获取Action描述
        /// </summary>
        public ApiActionDescriptor ApiAction => this.actionInvoker.ApiAction;

        /// <summary>
        /// 复合的ApiAction执行器
        /// 支持Task和ITask返回声明
        /// </summary>
        /// <param name="apiAction">Api描述</param>
        public MultiplexedActionInvoker(ApiActionDescriptor apiAction)
        {
            this.isTaskResult = apiAction.Return.ReturnType.IsInheritFrom<Task>();
            this.actionInvoker = new ActionInvoker<TResult>(apiAction);
        }

        /// <summary>
        /// 执行Action
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="arguments">参数值</param>
        /// <returns></returns>
        public object Invoke(HttpClientContext context, object?[] arguments)
        {
            return this.isTaskResult == true
                ? this.actionInvoker.Invoke(context, arguments)
                : new ActionTask<TResult>(this.actionInvoker, context, arguments);
        }
    }
}
