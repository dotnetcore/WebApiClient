using System;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 提供异常处理的请求任务
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    class ApiHandleTask<TResult> : ApiTask<TResult>, IHandleTask<TResult>
    {
        /// <summary>
        /// 请求任务创建的委托
        /// </summary>
        private readonly Func<Task<TResult>> invoker;

        /// <summary>
        /// 异常处理的请求任务
        /// </summary>
        /// <param name="invoker">请求任务创建的委托</param>
        public ApiHandleTask(Func<Task<TResult>> invoker)
        {
            this.invoker = invoker;
        }

        /// <summary>
        /// 创建请求任务
        /// </summary>
        /// <returns></returns>
        public override async Task<TResult> InvokeAsync()
        {
            return await this.invoker.Invoke().ConfigureAwait(false);
        }

        /// <summary>
        /// 当捕获到异常时返回指定结果
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="func">获取结果</param>
        /// <returns></returns>
        public IHandleTask<TResult> WhenCatch<TException>(Func<TResult> func) where TException : Exception
        {
            return this.WhenCatch<TException>(ex => func());
        }

        /// <summary>
        /// 当捕获到异常时返回指定结果
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="func">获取结果</param>
        /// <returns></returns>
        public IHandleTask<TResult> WhenCatch<TException>(Func<TException, TResult> func) where TException : Exception
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            async Task<TResult> newInvoker()
            {
                try
                {
                    return await this.invoker.Invoke().ConfigureAwait(false);
                }
                catch (TException ex)
                {
                    return func.Invoke(ex);
                }
            }

            return new ApiHandleTask<TResult>(newInvoker);
        }

        /// <summary>
        /// 当捕获到异常时返回指定结果
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="func">获取结果</param>
        /// <returns></returns>
        public IHandleTask<TResult> WhenCatchAsync<TException>(Func<TException, Task<TResult>> func) where TException : Exception
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            async Task<TResult> newInvoker()
            {
                try
                {
                    return await this.invoker.Invoke().ConfigureAwait(false);
                }
                catch (TException ex)
                {
                    return await func.Invoke(ex).ConfigureAwait(false);
                }
            }

            return new ApiHandleTask<TResult>(newInvoker);
        }
    }
}
