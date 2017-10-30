using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 表示支持重试的Api请求
    /// </summary>
    /// <typeparam name="TResult">结果类型</typeparam>
    class ApiRetryTask<TResult> : IRetryTask<TResult>
    {
        /// <summary>
        /// 任务执行器
        /// </summary>
        private Func<Task<TResult>> invoker;

        /// <summary>
        /// 获取最大重试次数
        /// </summary>
        private readonly int retryMaxCount;

        /// <summary>
        /// 获取各次重试的延时时间
        /// </summary>
        private readonly Func<int, TimeSpan> retryDelay;

        /// <summary>
        /// 支持重试的Api请求
        /// </summary>
        /// <param name="invoker">任务执行器</param>
        /// <param name="retryMaxCount">最大尝试次数</param>
        /// <param name="retryDelay">各次重试的延时时间</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public ApiRetryTask(Func<Task<TResult>> invoker, int retryMaxCount, Func<int, TimeSpan> retryDelay)
        {
            if (retryMaxCount < 1)
            {
                throw new ArgumentOutOfRangeException("retryMaxCount");
            }
            this.invoker = invoker;
            this.retryMaxCount = retryMaxCount;
            this.retryDelay = retryDelay;
        }

        /// <summary>
        /// 返回TaskAwaiter对象
        /// </summary>
        /// <returns></returns>
        public TaskAwaiter<TResult> GetAwaiter()
        {
            return this.InvokeAsync().GetAwaiter();
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <returns></returns>
        Task ITask.InvokeAsync()
        {
            return this.InvokeAsync();
        }

        /// <summary>
        /// 执行任务
        /// </summary>
        /// <returns></returns>
        public async Task<TResult> InvokeAsync()
        {
            var exception = default(Exception);
            for (var i = 0; i <= this.retryMaxCount; i++)
            {
                try
                {
                    await this.DelayBeforRetryAsync(i);
                    return await this.invoker.Invoke();
                }
                catch (RetryException ex)
                {
                    exception = ex.InnerException;
                }
            }

            if (exception == null)
            {
                exception = new ResultNotMatchException();
            }
            throw exception;
        }

        /// <summary>
        /// 执行前延时
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private async Task DelayBeforRetryAsync(int index)
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
            var target = this.invoker;
            this.invoker = async () =>
            {
                try
                {
                    return await target.Invoke();
                }
                catch (TException ex)
                {
                    if (predicate == null || predicate.Invoke(ex))
                    {
                        throw new RetryException(ex);
                    }
                    throw ex;
                }
            };
            return this;
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
                throw new ArgumentNullException();
            }

            var target = this.invoker;
            this.invoker = async () =>
            {
                var result = await target.Invoke();
                if (predicate.Invoke(result) == true)
                {
                    throw new RetryException(null);
                }
                return result;
            };
            return this;
        }

        /// <summary>
        /// 表示需要重试的异常
        /// </summary>
        private class RetryException : Exception
        {
            /// <summary>
            /// 需要重试的异常
            /// </summary>
            /// <param name="inner">内部异常</param>
            public RetryException(Exception inner)
                : base(null, inner)
            {
            }
        }
    }
}
