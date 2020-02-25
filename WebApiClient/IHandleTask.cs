using System;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// Define exception handling behavior
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IHandleTask<TResult> : ITask<TResult>
    {
        /// <summary>
        /// Returns the specified result when an exception is caught
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="func">Get results</param>
        /// <returns></returns>
        IHandleTask<TResult> WhenCatch<TException>(Func<TResult> func) where TException : Exception;

        /// <summary>
        /// Returns the specified result when an exception is caught
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="func">Get results</param>
        /// <returns></returns>
        IHandleTask<TResult> WhenCatch<TException>(Func<TException, TResult> func) where TException : Exception;

        /// <summary>
        /// Returns the specified result when an exception is caught
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="func">Get results</param>
        /// <returns></returns>
        IHandleTask<TResult> WhenCatchAsync<TException>(Func<TException, Task<TResult>> func) where TException : Exception;
    }
}
