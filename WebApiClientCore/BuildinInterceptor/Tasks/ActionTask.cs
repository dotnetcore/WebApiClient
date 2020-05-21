using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示Api请求的任务
    /// </summary>
    /// <typeparam name="TResult">结果类型</typeparam>
    class ActionTask<TResult> : TaskBase<TResult>
    {
        private readonly ActionInvoker<TResult> invoker;
        private readonly ServiceContext context;
        private readonly object?[] arguments;

        /// <summary>
        /// Api请求的任务
        /// </summary>       
        /// <param name="invoker"></param>
        /// <param name="context"></param>
        /// <param name="arguments"></param>
        public ActionTask(IActionInvoker invoker, ServiceContext context, object?[] arguments)
        {
            this.invoker = (ActionInvoker<TResult>)invoker;
            this.context = context;
            this.arguments = arguments;
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <returns></returns>
        public override Task<TResult> InvokeAsync()
        {
            return this.invoker.InvokeAsync(this.context, this.arguments);
        }
    }
}
