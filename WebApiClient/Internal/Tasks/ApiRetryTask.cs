using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示支持重试的Api请求任务
    /// </summary>
    /// <typeparam name="TResult">结果类型</typeparam>
    class ApiRetryTask<TResult> : IRetryTask<TResult>
    {
        /// <summary>
        /// 请求任务创建的委托
        /// </summary>
        private readonly Func<Task<TResult>> invoker;

        /// <summary>
        /// 获取最大重试次数
        /// </summary>
        private readonly int retryMaxCount;

        /// <summary>
        /// 获取各次重试的延时时间
        /// </summary>
        private readonly Func<int, TimeSpan> retryDelay;

        /// <summary>
        /// 支持重试的Api请求任务
        /// </summary>
        /// <param name="invoker">请求任务创建的委托</param>
        /// <param name="retryMaxCount">最大尝试次数</param>
        /// <param name="retryDelay">各次重试的延时时间</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ApiRetryTask(Func<Task<TResult>> invoker, int retryMaxCount, Func<int, TimeSpan> retryDelay)
        {
            if (retryMaxCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(retryMaxCount));
            }
            this.invoker = invoker;
            this.retryMaxCount = retryMaxCount;
            this.retryDelay = retryDelay;
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
        /// 创建请求任务
        /// </summary>
        /// <returns></returns>
        public async Task<TResult> InvokeAsync()
        {
            var inner = default(Exception);
            for (var i = 0; i <= this.retryMaxCount; i++)
            {
                try
                {
                    await this.DelayBeforRetry(i);
                    return await this.invoker.Invoke();
                }
                catch (RetryMarkException ex)
                {
                    inner = ex.InnerException;
                }
            }

            var message = $"已经重试了{this.retryMaxCount}次，但结果仍未正确";
            throw new RetryException(message, inner);
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
                await Task.Delay(delay);
            }
        }

        /// <summary>
        /// 当捕获到异常时进行Retry
        /// </summary>
        /// <typeparam name="TException">异常类型</typeparam>
        /// <returns></returns>
        public IRetryTask<TResult> WhenCatch<TException>() where TException : Exception
        {
            return this.WhenCatch<TException>(null);
        }

        /// <summary>
        /// 当捕获到异常时进行Retry
        /// </summary>
        /// <typeparam name="TException">异常类型</typeparam>
        /// <param name="predicate">返回true才Retry</param>
        /// <returns></returns>
        public IRetryTask<TResult> WhenCatch<TException>(Func<TException, bool> predicate) where TException : Exception
        {
            async Task<TResult> newInvoker()
            {
                try
                {
                    return await this.invoker.Invoke();
                }
                catch (TException ex)
                {
                    if (predicate == null || predicate.Invoke(ex))
                    {
                        throw new RetryMarkException(ex);
                    }
                    throw ex;
                }
            }
            return new ApiRetryTask<TResult>(newInvoker, this.retryMaxCount, this.retryDelay);
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

            async Task<TResult> newInvoker()
            {
                var result = await this.invoker.Invoke();
                if (predicate.Invoke(result) == true)
                {
                    var inner = new ResultNotMatchException("结果不符合预期值", result);
                    throw new RetryMarkException(inner);
                }
                return result;
            }

            return new ApiRetryTask<TResult>(newInvoker, this.retryMaxCount, this.retryDelay);
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
