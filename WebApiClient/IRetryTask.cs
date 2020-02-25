using System;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// Define retry behavior
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IRetryTask<TResult> : ITask<TResult>
    {
        /// <summary>
        /// Retry when an exception is caught
        /// </summary>
        /// <typeparam name="TException">Exception type</typeparam>
        /// <returns></returns>
        IRetryTask<TResult> WhenCatch<TException>() where TException : Exception;

        /// <summary>
        /// Retry when an exception is caught
        /// </summary>
        /// <typeparam name="TException">Exception type</typeparam>
        /// <param name="handler">When the specified exception is caught</param>
        /// <returns></returns>
        IRetryTask<TResult> WhenCatch<TException>(Action<TException> handler) where TException : Exception;

        /// <summary>
        /// Retry when an exception is caught
        /// </summary>
        /// <typeparam name="TException">Exception type</typeparam>
        /// <param name="predicate">Return true before Retry</param>
        /// <returns></returns>
        IRetryTask<TResult> WhenCatch<TException>(Func<TException, bool> predicate) where TException : Exception;

        /// <summary>
        /// Retry when an exception is caught
        /// </summary>
        /// <typeparam name="TException">Exception type</typeparam>
        /// <param name="handler">When the specified exception is caught</param>
        /// <returns></returns>
        IRetryTask<TResult> WhenCatchAsync<TException>(Func<TException, Task> handler) where TException : Exception;

        /// <summary>
        /// Retry when an exception is caught
        /// </summary>
        /// <typeparam name="TException">Exception type</typeparam>
        /// <param name="predicate">condition</param>
        /// <returns></returns>
        IRetryTask<TResult> WhenCatchAsync<TException>(Func<TException, Task<bool>> predicate) where TException : Exception;

        /// <summary>
        /// Retry when an exception is caught
        /// </summary>
        /// <param name="predicate">condition</param>
        /// <returns></returns>
        IRetryTask<TResult> WhenResult(Func<TResult, bool> predicate);

        /// <summary>
        /// Retry when an exception is caught
        /// </summary>
        /// <param name="predicate">condition</param>
        /// <returns></returns>
        IRetryTask<TResult> WhenResultAsync(Func<TResult, Task<bool>> predicate);
    }
}
