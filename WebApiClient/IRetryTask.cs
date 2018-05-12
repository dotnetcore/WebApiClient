using System;

namespace WebApiClient
{
    /// <summary>
    /// 定义重试的行为
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public interface IRetryTask<TResult> : ITask<TResult>
    {
        /// <summary>
        /// 当捕获到异常时进行Retry
        /// </summary>
        /// <typeparam name="TException">异常类型</typeparam>
        /// <returns></returns>
        IRetryTask<TResult> WhenCatch<TException>() where TException : Exception;

        /// <summary>
        /// 当捕获到异常时进行Retry
        /// </summary>
        /// <typeparam name="TException">异常类型</typeparam>
        /// <param name="predicate">返回true才Retry</param>
        /// <returns></returns>
        IRetryTask<TResult> WhenCatch<TException>(Func<TException, bool> predicate) where TException : Exception;

        /// <summary>
        /// 当结果符合条件时进行Retry
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        IRetryTask<TResult> WhenResult(Func<TResult, bool> predicate);
    }
}
