using System;
using System.Threading.Tasks;
using WebApiClientCore.Exceptions;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示支持重试的Api请求任务
    /// </summary>
    /// <typeparam name="TResult">结果类型</typeparam>
    class AcitonRetryTask<TResult> : TaskBase<TResult>, IRetryTask<TResult>
    {
        /// <summary>
        /// 请求任务创建的委托
        /// </summary>
        private readonly Func<Task<TResult>> invoker;

        /// <summary>
        /// 获取最大重试次数
        /// </summary>
        private readonly int maxRetryCount;

        /// <summary>
        /// 获取各次重试的延时时间
        /// </summary>
        private readonly Func<int, TimeSpan>? retryDelay;

        /// <summary>
        /// 支持重试的Api请求任务
        /// </summary>
        /// <param name="invoker">请求任务创建的委托</param>
        /// <param name="maxRetryCount">最大尝试次数</param>
        /// <param name="retryDelay">各次重试的延时时间</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public AcitonRetryTask(Func<Task<TResult>> invoker, int maxRetryCount, Func<int, TimeSpan>? retryDelay)
        {
            if (maxRetryCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maxRetryCount));
            }
            this.invoker = invoker;
            this.maxRetryCount = maxRetryCount;
            this.retryDelay = retryDelay;
        }

        /// <summary>
        /// 创建新的请求任务
        /// </summary>
        /// <returns></returns>
        protected override async Task<TResult> InvokeAsync()
        {
            var inner = default(Exception);
            for (var i = 0; i <= this.maxRetryCount; i++)
            {
                try
                {
                    await this.DelayBeforRetry(i).ConfigureAwait(false);
                    return await this.invoker.Invoke().ConfigureAwait(false);
                }
                catch (RetryMarkException ex)
                {
                    inner = ex.InnerException;
                }
            }

            throw new ApiRetryException(this.maxRetryCount, inner);
        }

        /// <summary>
        /// 执行前延时
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private async Task DelayBeforRetry(int index)
        {
            if (index == 0 || this.retryDelay == null)
            {
                return;
            }

            var retryIndex = index - 1;
            var delay = this.retryDelay(retryIndex);

            if (delay > TimeSpan.Zero)
            {
                await Task.Delay(delay).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 当捕获到异常时进行Retry
        /// </summary>
        /// <typeparam name="TException">异常类型</typeparam>
        /// <returns></returns>
        public IRetryTask<TResult> WhenCatch<TException>() where TException : Exception
        {
            return this.WhenCatch<TException>(ex => true);
        }

        /// <summary>
        /// 当捕获到异常时进行Retry
        /// </summary>
        /// <typeparam name="TException">异常类型</typeparam>
        /// <param name="handler">捕获到指定异常时</param>
        /// <returns></returns>
        public IRetryTask<TResult> WhenCatch<TException>(Action<TException> handler) where TException : Exception
        {
            return this.WhenCatch<TException>(ex =>
            {
                handler?.Invoke(ex);
                return true;
            });
        }

        /// <summary>
        /// 当捕获到异常时进行Retry
        /// </summary>
        /// <typeparam name="TException">异常类型</typeparam>
        /// <param name="predicate">返回true才Retry</param>
        /// <returns></returns>
        public IRetryTask<TResult> WhenCatch<TException>(Func<TException, bool> predicate) where TException : Exception
        {
            return this.WhenCatchAsync<TException>(ex =>
            {
                var result = predicate == null || predicate.Invoke(ex);
                return Task.FromResult(result);
            });
        }

        /// <summary>
        /// 当捕获到异常时进行Retry
        /// </summary>
        /// <typeparam name="TException">异常类型</typeparam>
        /// <param name="handler">捕获到指定异常时</param>
        /// <returns></returns>
        public IRetryTask<TResult> WhenCatchAsync<TException>(Func<TException, Task> handler) where TException : Exception
        {
            return this.WhenCatchAsync<TException>(async ex =>
            {
                if (handler != null)
                {
                    await handler.Invoke(ex).ConfigureAwait(false);
                }
                return true;
            });
        }

        /// <summary>
        /// 当捕获到异常时进行Retry
        /// </summary>
        /// <typeparam name="TException">异常类型</typeparam>
        /// <param name="predicate">返回true才Retry</param>
        /// <returns></returns>
        public IRetryTask<TResult> WhenCatchAsync<TException>(Func<TException, Task<bool>> predicate) where TException : Exception
        {
            async Task<TResult> newInvoker()
            {
                try
                {
                    return await this.invoker.Invoke().ConfigureAwait(false);
                }
                catch (TException ex)
                {
                    if (predicate == null || await predicate.Invoke(ex).ConfigureAwait(false))
                    {
                        throw new RetryMarkException(ex);
                    }
                    throw;
                }
            }
            return new AcitonRetryTask<TResult>(newInvoker, this.maxRetryCount, this.retryDelay);
        }

        /// <summary>
        /// 当结果符合条件时进行Retry
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        public IRetryTask<TResult> WhenResult(Func<TResult, bool> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            return this.WhenResultAsync(r =>
            {
                var result = predicate.Invoke(r);
                return Task.FromResult(result);
            });
        }

        /// <summary>
        /// 当结果符合条件时进行Retry
        /// </summary>
        /// <param name="predicate">条件</param>
        /// <returns></returns>
        public IRetryTask<TResult> WhenResultAsync(Func<TResult, Task<bool>> predicate)
        {
            if (predicate == null)
            {
                throw new ArgumentNullException(nameof(predicate));
            }

            async Task<TResult> newInvoker()
            {
                var result = await this.invoker.Invoke().ConfigureAwait(false);
                if (await predicate.Invoke(result).ConfigureAwait(false) == true)
                {
                    var inner = new ApiResultNotMatchException(Resx.unexpected_Result, result);
                    throw new RetryMarkException(inner);
                }
                return result;
            }

            return new AcitonRetryTask<TResult>(newInvoker, this.maxRetryCount, this.retryDelay);
        }

        /// <summary>
        /// 表示重试标记的异常
        /// </summary>
        private class RetryMarkException : Exception
        {
            /// <summary>
            /// 重试标记的异常
            /// </summary>
            /// <param name="inner">内部异常</param>
            public RetryMarkException(Exception inner)
                : base(null, inner)
            {
            }
        }
    }
}
