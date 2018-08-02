using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 定义返回结果的行为
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface ITask<TResult>
    {
        /// <summary>
        /// 创建请求任务
        /// </summary>
        /// <returns></returns>
        Task<TResult> InvokeAsync();

        /// <summary>
        /// 调用InvokeAsync
        /// 并返回其TaskAwaiter对象
        /// </summary>
        /// <returns></returns>
        TaskAwaiter<TResult> GetAwaiter();

        /// <summary>
        /// 配置用于等待的等待者
        /// </summary>
        /// <param name="continueOnCapturedContext">试图继续回夺取的原始上下文，则为 true；否则为 false</param>
        /// <returns></returns>
        ConfiguredTaskAwaitable<TResult> ConfigureAwait(bool continueOnCapturedContext);
    }
}
