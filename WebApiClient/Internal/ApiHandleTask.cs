using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WebApiClient
{
    /// <summary>
    /// 提供异常处理的请求任务
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    class ApiHandleTask<TResult> : IHandleTask<TResult>
    {
        /// <summary>
        /// 请求任务创建的委托
        /// </summary>
        private Func<Task<TResult>> invoker;

        /// <summary>
        /// 异常处理的请求任务
        /// </summary>
        /// <param name="invoker">请求任务创建的委托</param>
        public ApiHandleTask(Func<Task<TResult>> invoker)
        {
            this.invoker = invoker;
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
            return await this.invoker.Invoke();
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
                throw new ArgumentNullException();
            }

            var inner = this.invoker;
            this.invoker = async () =>
            {
                try
                {
                    return await inner.Invoke();
                }
                catch (TException ex)
                {
                    return func.Invoke(ex);
                }
            };
            return this;
        }

        /// <summary>
        /// 当捕获到异常时返回指定结果
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="func">获取结果</param>
        /// <returns></returns>
        public IHandleTask<TResult> WhenCatch<TException>(Func<TException, Task<TResult>> func) where TException : Exception
        {
            if (func == null)
            {
                throw new ArgumentNullException();
            }

            var inner = this.invoker;
            this.invoker = async () =>
            {
                TException _ex;
                try
                {
                    return await inner.Invoke();
                }
                catch (TException ex)
                {
                    _ex = ex;
                }
                return await func.Invoke(_ex);
            };
            return this;
        }
    }
}
