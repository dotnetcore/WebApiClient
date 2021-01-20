using System.Threading.Tasks;

namespace WebApiClientCore.Implementations.Tasks
{
    /// <summary>
    /// 表示Api请求的任务
    /// </summary>
    /// <typeparam name="TResult">结果类型</typeparam>
    sealed class ActionTask<TResult> : TaskBase<TResult>
    {
        private readonly TaskApiActionInvoker<TResult> invoker;
        private readonly HttpClientContext context;
        private readonly object?[] arguments;

        /// <summary>
        /// Api请求的任务
        /// </summary>       
        /// <param name="invoker"></param>
        /// <param name="context"></param>
        /// <param name="arguments"></param>
        public ActionTask(TaskApiActionInvoker<TResult> invoker, HttpClientContext context, object?[] arguments)
        {
            this.invoker = invoker;
            this.context = context;
            this.arguments = arguments;
        }

        /// <summary>
        /// 创建新的请求任务
        /// </summary>
        /// <returns></returns>
        protected override Task<TResult> InvokeAsync()
        {
            return this.invoker.InvokeAsync(this.context, this.arguments);
        }
    }
}
