using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示Task抽象类
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    abstract class TaskBase<TResult> : ITask<TResult>
    {
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
        public abstract Task<TResult> InvokeAsync();
    }
}
