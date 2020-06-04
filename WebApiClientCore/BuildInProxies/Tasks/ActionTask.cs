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
        private readonly HttpClientContext context;
        private readonly object?[] arguments;

        /// <summary>
        /// Api请求的任务
        /// </summary>       
        /// <param name="invoker"></param>
        /// <param name="context"></param>
        /// <param name="arguments"></param>
        public ActionTask(ActionInvoker<TResult> invoker, HttpClientContext context, object?[] arguments)
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
