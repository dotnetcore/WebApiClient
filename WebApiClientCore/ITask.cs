using System.Runtime.CompilerServices;

namespace WebApiClientCore
{
    /// <summary>
    /// 定义返回结果的行为
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface ITask<TResult>
    {
        /// <summary>
        /// 返回新创建的请求任务的等待器
        /// </summary>
        /// <returns></returns>
        TaskAwaiter<TResult> GetAwaiter();

        /// <summary>
        /// 返回新创建的请求任务的等待器
        /// </summary>
        /// <param name="continueOnCapturedContext">试图继续回夺取的原始上下文，则为 true；否则为 false</param>
        /// <returns></returns>
        ConfiguredTaskAwaitable<TResult> ConfigureAwait(bool continueOnCapturedContext);
    }
}
