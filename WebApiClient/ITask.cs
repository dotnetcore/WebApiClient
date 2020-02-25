using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// Define the behavior of returning results
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// Create Request Task
        /// </summary>
        /// <returns></returns>
        Task InvokeAsync();
    }

    /// <summary>
    /// Define the behavior of returning results
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface ITask<TResult> : ITask
    {
        /// <summary>
        /// Create Request Task
        /// </summary>
        /// <returns></returns>
        new Task<TResult> InvokeAsync();

        /// <summary>
        /// Call InvokeAsync
        /// And return its TaskAwaiter object
        /// </summary>
        /// <returns></returns>
        TaskAwaiter<TResult> GetAwaiter();

        /// <summary>
        /// Configure waiters for waiting
        /// </summary>
        /// <param name="continueOnCapturedContext">Attempt to resume the original context captured，则为 true；否则为 false</param>
        /// <returns></returns>
        ConfiguredTaskAwaitable<TResult> ConfigureAwait(bool continueOnCapturedContext);
    }
}
