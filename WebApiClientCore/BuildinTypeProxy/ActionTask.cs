using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示Api请求的任务
    /// </summary>
    /// <typeparam name="TResult">结果类型</typeparam>
    class ActionTask<TResult> : ITask<TResult>
    {
        private readonly ActionInvoker<TResult> invoker;
        private readonly ServiceContext context;
        private readonly object[] arguments;

        /// <summary>
        /// Api请求的任务
        /// </summary>
        protected ActionTask()
        {
        }

        /// <summary>
        /// Api请求的任务
        /// </summary>       
        /// <param name="invoker"></param>
        /// <param name="context"></param>
        /// <param name="arguments"></param>
        public ActionTask(IActionInvoker invoker, ServiceContext context, object[] arguments)
        {
            this.invoker = (ActionInvoker<TResult>)invoker;
            this.context = context;
            this.arguments = arguments;
        }

        /// <summary>
        /// 执行InvokeAsync
        /// 并返回其TaskAwaiter对象
        /// </summary>
        /// <returns></returns>
        public TaskAwaiter<TResult> GetAwaiter()
        {
            return this.InvokeAsync().GetAwaiter();
        }

        /// <summary>
        /// 配置用于等待的等待者
        /// </summary>
        /// <param name="continueOnCapturedContext">试图继续回夺取的原始上下文，则为 true；否则为 false</param>
        /// <returns></returns>
        public ConfiguredTaskAwaitable<TResult> ConfigureAwait(bool continueOnCapturedContext)
        {
            return this.InvokeAsync().ConfigureAwait(continueOnCapturedContext);
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <returns></returns>
        public virtual Task<TResult> InvokeAsync()
        {
            return this.invoker.InvokeAsync(this.context, this.arguments);
        }
    }
}
